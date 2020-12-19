using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Proggy.Core;
using Proggy.Infrastructure.Converters.Json;

namespace Proggy.Infrastructure
{
    public static class ClickTrackFile
    {
        public const string FileExtension = ".json";

        public static string FolderPath { get; }

        private static readonly JsonSerializerOptions options;

        static ClickTrackFile()
        {
            options = new JsonSerializerOptions();
            options.Converters.Add(new BarInfoConverter());

            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = $"{Constants.AppName}\\Tracks";
            FolderPath = Path.Combine(appDataFolder, appFolder);

            EnsureDirectory();
        }

        public static async Task Save(BarInfo[] data, string filename)
        {
            EnsureDirectory();

            var bytes = JsonSerializer.SerializeToUtf8Bytes(data, options);
            var filePath = Path.Combine(FolderPath, filename + FileExtension);

            using var stream = File.Create(filePath);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public static async Task<BarInfo[]> Load(string filename)
        {
            var filePath = Path.Combine(FolderPath, filename + FileExtension);

            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<BarInfo[]>(stream, options);
        }

        public static string[] Enumerate()
        {
            EnsureDirectory();

            return Directory.GetFiles(FolderPath, $"*{FileExtension}")
                            .Select(Path.GetFileNameWithoutExtension)
                            .ToArray();
        }

        public static void Delete(string filename)
        {
            var filePath = Path.Combine(FolderPath, filename + FileExtension);

            File.Delete(filePath);
        }

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
        }
    }
}
