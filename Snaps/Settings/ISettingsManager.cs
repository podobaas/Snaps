using System;

namespace Snaps.Settings
{
    public interface ISettingsManager
    {
        public TSettings Read<TSettings>();

        public void Update<TSettings>(TSettings settings);
    }
}