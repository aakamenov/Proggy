using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reactive.Linq;
using NAudio.Wave.SampleProviders;
using Material.Styles.Themes;
using Akavache;
using Proggy.Core;

namespace Proggy.Models
{
    public sealed class UserSettings
    {
        private const string Key = "user-settings";

        public ClickSettings ClickSettings { get; }
        public ITheme Theme { get; set; }

        private UserSettings() 
        {
            ClickSettings = new ClickSettings()
            {
                WaveType = SignalGeneratorType.Sin,
                AccentClickFreq = 4000,
                ClickFreq = 2000
            };

            Theme = new PaletteHelper().GetTheme();
        }

        public static async Task<UserSettings> Get()
        {
            try
            {
                var settings = await BlobCache.LocalMachine.GetObject<UserSettings>(Key);
                return settings;
            }
            catch (KeyNotFoundException)
            {
                var settings = new UserSettings();
                await settings.Save();

                return settings;
            }
        }

        public async Task Save()
        {
            await BlobCache.LocalMachine.InsertObject(Key, this);
        }
    }
}
