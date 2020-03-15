using System;
using System.IO;
using Newtonsoft.Json;

namespace Snaps.Settings
{
    public class SettingsManager: ISettingsManager
    {
        public TSettings Read<TSettings>()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "settings.json");
            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<TSettings>(json);
        }

        public void Update<TSettings>(TSettings settings)
        {
            var file = Path.Combine(AppContext.BaseDirectory, "settings.json");
            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(file, json);
        }
    }
}