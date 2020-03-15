using System;
using Microsoft.Extensions.CommandLineUtils;
using Snaps.Settings;

namespace Snaps.Commands
{
    public class SettingsCmdCommand: ICmdCommand
    {
        private readonly CommandLineApplication _commandLineApplication;
        private readonly ISettingsManager _settingsManager;
        private readonly SnapsSettings _settings;
        
        public SettingsCmdCommand(CommandLineApplication commandLineApplication, ISettingsManager settingsManager)
        {
            _commandLineApplication = commandLineApplication;
            _settingsManager = settingsManager;
            _settings = settingsManager.Read<SnapsSettings>();
        }
        
        public void Build()
        {
            _commandLineApplication.Command("settings", command =>
            {
                command.Description = "Settings";
                command.HelpOption("-?|-h|--help");
                var tokenOption = command.Option("-t|--token", "Digital Ocean API token", CommandOptionType.SingleValue);
                var maxConcurrencyOption = command.Option("-mc|--max-concurrency", "Max degree of concurrency for create snapshot (default 5)", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    if (tokenOption.HasValue())
                    {
                        _settings.Token = tokenOption.Value();
                        _settingsManager.Update(_settings);
                    }
                    
                    if (maxConcurrencyOption.HasValue())
                    {
                        var result = int.TryParse(maxConcurrencyOption.Value(), out var number);
                        
                        if (result)
                        {
                            _settings.MaxDegreeOfConcurrency = number;
                            _settingsManager.Update(_settings);
                        }
                        else
                        {
                            Console.WriteLine($"Value {maxConcurrencyOption.Value()} is not numerical");
                        }
                    }

                    return 0;
                });
                
            });
        }
    }
}