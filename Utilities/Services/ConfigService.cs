using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utilities.Services
{
    public interface IConfigService<ConfigType>
    {
        public void SaveConfigCopy(ConfigType config, string suggestedFileName);
        public void SaveConfig(ConfigType config, string suggestedFileName);
        public ConfigType? LoadConfig(string? path = null);
    }

    public class ConfigService<ConfigType>(string root) : IConfigService<ConfigType>
    {
        private JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
        };

        public ConfigType? LoadConfig(string? path = null)
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

        private ConfigType? LoadConfigImpl(string path)
        {
            FileName = path;
            var raw = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return default;
            }

            return JsonSerializer.Deserialize<ConfigType>(raw, SerializerOptions);
        }

        private string FileName { get; set; } = string.Empty;

        public void SaveConfigCopy(ConfigType config, string suggestedFileName)
        {
            var dialog = new SaveFileDialog()
            {
                InitialDirectory = root,
                FileName = suggestedFileName,
                Filter = "JSON|*.json",
            };

            bool? ok = dialog.ShowDialog();
            if (ok == true)
            {
                SaveConfigImpl(config, dialog.FileName);
            }
        }

        public void SaveConfig(ConfigType config, string suggestedFileName)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                SaveConfigImpl(config, FileName);
            }
            else
            {
                SaveConfigCopy(config, suggestedFileName);
            }
        }

        private void SaveConfigImpl(ConfigType config, string path)
        {
            FileName = path;
            File.WriteAllText(path, JsonSerializer.Serialize(config, SerializerOptions));
        }
    }
}