using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Snaps.Commands;

namespace Snaps
{
    public class SnapsApplication
    {
        private readonly CommandLineApplication _commandLineApplication;
        private readonly IEnumerable<ICmdCommand> _cmdCommands;
        
        public SnapsApplication(CommandLineApplication commandLineApplication, IEnumerable<ICmdCommand> cmdCommands)
        {
            _commandLineApplication = commandLineApplication;
            _cmdCommands = cmdCommands;
        }
        
        public void Run(string[] args)
        {
            _commandLineApplication.Name = "snaps";
            _commandLineApplication.HelpOption("-?|-h|--help");
            
            foreach (var cmdCommand in _cmdCommands)
            {
                cmdCommand.Build();
            }
            
            _commandLineApplication.Execute(args);
        }
    }
}