using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrimesterPlaner.Models;

namespace TrimesterPlaner.Services
{
    public interface IConfigService
    {
        public void SaveConfigCopy(Config config);
        public void SaveConfig(Config config);
        public Config? LoadConfig(string? path = null);
    }

    public class ConfigService(string root) : IConfigService
    {
        private JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
        };

        public Config? LoadConfig(string? path = null)
        {
            if (path is not null)
            {
                return LoadConfigImpl(path);
            }

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = root,
                Filter = "JSON|*.json",
            };

            bool? ok = dialog.ShowDialog();
            if (ok != true || !File.Exists(dialog.FileName))
            {
                return default;
            }

            return LoadConfigImpl(dialog.FileName);
        }

        private Config? LoadConfigImpl(string path)
        {
            FileName = path;
            var raw = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return default;
            }

            return JsonSerializer.Deserialize<Config>(raw, SerializerOptions);
        }

        private string FileName { get; set; } = string.Empty;

        public void SaveConfigCopy(Config config)
        {
            var dialog = new SaveFileDialog()
            {
                InitialDirectory = root,
                FileName = "TrimesterPlaner",
                Filter = "JSON|*.json",
            };

            bool? ok = dialog.ShowDialog();
            if (ok == true)
            {
                SaveConfigImpl(config, dialog.FileName);
            }
        }

        public void SaveConfig(Config config)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                SaveConfigImpl(config, FileName);
            }
            else
            {
                SaveConfigCopy(config);
            }
        }

        private void SaveConfigImpl(Config config, string path)
        {
            FileName = path;
            File.WriteAllText(path, JsonSerializer.Serialize(config, SerializerOptions));
        }
    }
}