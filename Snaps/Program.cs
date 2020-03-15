using DigitalOcean.API;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Snaps.Commands;
using Snaps.Settings;

namespace Snaps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = ConfigureServices();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<SnapsApplication>().Run(args);
        }

        private static IServiceCollection ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<CommandLineApplication>();
            serviceCollection.AddSingleton<SnapsApplication>();
            serviceCollection.AddSingleton<ISettingsManager, SettingsManager>();
            serviceCollection.AddSingleton<ICmdCommand, SettingsCmdCommand>();
            serviceCollection.AddSingleton<ICmdCommand, ListCmdCommand>();
            serviceCollection.AddSingleton<ICmdCommand, SnapshotCmdCommand>();
            serviceCollection.AddSingleton<IDigitalOceanClient, DigitalOceanClient>(provider =>
            {
                var settingsManager = provider.GetService<ISettingsManager>();
                var settings = settingsManager.Read<SnapsSettings>();
                return new DigitalOceanClient(settings.Token);
            });
            return serviceCollection;
        }
    }
}