using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalOcean.API;
using DigitalOcean.API.Models.Requests;
using Microsoft.Extensions.CommandLineUtils;
using Snaps.Settings;

namespace Snaps.Commands
{
    public class SnapshotCmdCommand : ICmdCommand
    {
        private readonly CommandLineApplication _commandLineApplication;
        private readonly IDigitalOceanClient _digitalOceanClient;
        private readonly SnapsSettings _settings;

        public SnapshotCmdCommand(
            CommandLineApplication commandLineApplication, 
            ISettingsManager settingsManager , 
            IDigitalOceanClient digitalOceanClient)
        {
            _commandLineApplication = commandLineApplication;
            _digitalOceanClient = digitalOceanClient;
            _settings = settingsManager.Read<SnapsSettings>();
        }

        public void Build()
        {
            _commandLineApplication.Command("snapshot", command =>
                {
                    command.Description = "Create a snapshot from a droplet or volume";
                    command.HelpOption("-?|-h|--help");
                    var idsArg = command.Argument("[Ids]", "Ids droplet or volume", true);
                    var dropletsOption = command.Option("-d|--droplets", "List all your droplets", CommandOptionType.NoValue);
                    var volumesOption = command.Option("-v|--volumes", "List all your volumes", CommandOptionType.NoValue);
                    var fileOption = command.Option("-f|--file", "Get ids droplet or volume from csv file", CommandOptionType.SingleValue);

                    command.OnExecute(() =>
                    {
                        if (idsArg.Values.Count > 0 && fileOption.HasValue())
                        {
                            Console.WriteLine("Use snaps snapshot <ids> or snaps -f <path to file with ids>");
                            return 0;
                        }
                        
                        if (idsArg.Values.Count > 0 && dropletsOption.HasValue())
                        {
                            CreateSnapshots(idsArg.Values, id => CreateDropletSnapshotAsync(long.Parse(id)));
                        }

                        if (idsArg.Values.Count > 0 && volumesOption.HasValue())
                        {
                            CreateSnapshots(idsArg.Values, id => CreateVolumeSnapshotAsync(id));
                        }

                        if (fileOption.HasValue() && dropletsOption.HasValue())
                        {
                            var ids = File.ReadAllText(fileOption.Value()).Split(';');
                            CreateSnapshots(ids, id => CreateDropletSnapshotAsync(long.Parse(id)));
                        }
                        
                        if (fileOption.HasValue() && volumesOption.HasValue())
                        {
                            var ids = File.ReadAllText(fileOption.Value()).Split(';');
                            CreateSnapshots(ids, id => CreateVolumeSnapshotAsync(id));
                        }
                        
                        return 0;
                    });
                },
                false);
        }

        private void CreateSnapshots(IEnumerable<string> ids, Func<string, Task> func)
        {
            var count = 0;
            var tasks = new List<Task>();
            
            do
            {
                var selectedIds = ids.Skip(count).Take(_settings.MaxDegreeOfConcurrency);
                
                foreach (var id in selectedIds)
                {
                    var task = func(id);
                    tasks.Add(task);
                }
            
                Task.WaitAll(tasks.ToArray());

                count += selectedIds.Count();
                tasks.Clear();

            } 
            while (count != ids.Count());
        }

        private async Task CreateDropletSnapshotAsync(long id)
        {
            try
            {
                Console.WriteLine($"Creating snapshot for a droplet with id {id}");
                var response = await _digitalOceanClient.DropletActions.Snapshot(id, null);
                var result = await CheckDropletActionStatusAsync(id, response.Id).ConfigureAwait(false);
                if (!result)
                {
                    Console.WriteLine($"Snapshot for a droplet with id {id} has been complied with error");
                    return;
                }
                
                Console.WriteLine($"Snapshot has been created for a droplet with id {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the snapshot for a droplet with id {id}. Error: {ex.Message}");
            }
        }

        private async Task CreateVolumeSnapshotAsync(string id)
        {
            try
            {
                Console.WriteLine($"Creating snapshot for a volume with id {id}");
                await _digitalOceanClient.Volumes.CreateSnapshot(id, new VolumeSnapshot { Name = Guid.NewGuid().ToString() });
                Console.WriteLine($"Snapshot has been created for a droplet with id {id}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the snapshot for a volume with id {id}. Error: {ex.Message}");
            }
        }
        
        private async Task<bool> CheckDropletActionStatusAsync(long dropletId, long actionId)
        {
            while (true)
            {
                try
                {
                    var action = await _digitalOceanClient.DropletActions.GetDropletAction(dropletId, actionId);

                    switch (action.Status)
                    {
                        case "completed":
                            return true;
                        case "errored":
                            return false;
                        default:
                            await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while check status for a droplet with id {dropletId}. Error: {ex.Message}");
                }
            }
        }
    }
}