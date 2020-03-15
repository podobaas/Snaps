using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;
using DigitalOcean.API;
using Microsoft.Extensions.CommandLineUtils;

namespace Snaps.Commands
{
    public class ListCmdCommand: ICmdCommand
    {
        private readonly CommandLineApplication _commandLineApplication;
        private readonly IDigitalOceanClient _digitalOceanClient;
        
        public ListCmdCommand(CommandLineApplication commandLineApplication, IDigitalOceanClient digitalOceanClient)
        {
            _commandLineApplication = commandLineApplication;
            _digitalOceanClient = digitalOceanClient;
        }

        public void Build()
        {
            _commandLineApplication.Command("list", command =>
                {
                    command.Description = "List all droplets or volumes";
                    command.HelpOption("-?|-h|--help");
                    var dropletsOption = command.Option("-d|--droplets", "List all droplets", CommandOptionType.NoValue);
                    var volumesOption = command.Option("-v|--volumes", "List all volumes", CommandOptionType.NoValue);
                    var snapshotOption = command.Option("-s|--snapshots", "List all snapshots", CommandOptionType.NoValue);
                    var outputOption = command.Option("-o|--output", "Export ids a droplet or volume to csv file", CommandOptionType.SingleValue);

                    command.OnExecute(async () =>
                    {
                        if (dropletsOption.HasValue())
                        {
                            await GetDroplets(outputOption).ConfigureAwait(false);
                        }

                        if (volumesOption.HasValue())
                        {
                            await GetVolumes(outputOption).ConfigureAwait(false);
                        }

                        if (snapshotOption.HasValue())
                        {
                            await GetSnapshots().ConfigureAwait(false);
                        }

                        return 0;
                    });
                }, 
                false);
        }

        private async Task GetDroplets(CommandOption outputOption)
        {
            try
            {

                var droplets = await _digitalOceanClient.Droplets.GetAll();
                if (droplets.Count == 0)
                {
                    Console.WriteLine("You don't have droplets");
                    return;
                }

                if (outputOption.HasValue())
                {
                    var ids = string.Join(';', droplets.Select(d => d.Id));
                    File.WriteAllText(outputOption.Value(), ids);
                }

                var table = new ConsoleTable("ID", "Created at (UTC)", "Name", "Memory", "Disk", "Region").Configure(c => c.NumberAlignment = Alignment.Left);

                foreach (var droplet in droplets)
                {
                    table.AddRow(droplet.Id, droplet.CreatedAt.ToUniversalTime().ToString("f"), droplet.Name, droplet.Memory, droplet.Disk, droplet.Region.Name);
                }

                table.Write(Format.Alternative);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred. Error: {ex.Message}");
            }
        }

        private async Task GetVolumes(CommandOption outputOption)
        {
            try
            {

                var volumes = await _digitalOceanClient.Volumes.GetAll();
                if (volumes.Count == 0)
                {
                    Console.WriteLine("You don't have volumes");
                    return;
                }

                if (outputOption.HasValue())
                {
                    var ids = string.Join(';', volumes.Select(d => d.Id));
                    File.WriteAllText(outputOption.Value(), ids);
                }

                var table = new ConsoleTable("ID", "Created at (UTC)", "Name", "Disk", "Region", "Filesystem Type").Configure(c => c.NumberAlignment = Alignment.Left);

                foreach (var volume in volumes)
                {
                    table.AddRow(volume.Id, volume.CreatedAt.ToUniversalTime().ToString("f"), volume.Name, volume.SizeGigabytes.ToString(), volume.Region.Name, volume.FilesystemType);
                }

                table.Write(Format.Alternative);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred. Error: {ex.Message}");
            }
        }
        
        private async Task GetSnapshots()
        {
            try
            {
                var snapshots = await _digitalOceanClient.Snapshots.GetAll();
                if (snapshots.Count == 0)
                {
                    Console.WriteLine("You don't have snapshots");
                    return;
                }

                var table = new ConsoleTable("ID", "Created at (UTC)", "Name", "Size", "Resource type").Configure(c => c.NumberAlignment = Alignment.Left);

                foreach (var snapshot in snapshots)
                {
                    table.AddRow(snapshot.Id, snapshot.CreatedAt.ToUniversalTime().ToString("f"), snapshot.Name, snapshot.SizeGigabytes.ToString(), snapshot.ResourceType);
                }

                table.Write(Format.Alternative);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred. Error: {ex.Message}");
            }
        }
    }
}