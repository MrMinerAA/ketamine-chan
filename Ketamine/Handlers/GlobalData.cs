using Ayako.DataStructs;
using Ayako.Services;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ayako.Handlers
{
    public class GlobalData
    {
        public static string ConfigPath { get; set; } = "config.json";
        public static BotConfig Config { get; set; }

        public async Task InitializeAsync()
        {
            if (!File.Exists(ConfigPath))
            {
                Config = GenerateNewConfig();
                string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                await File.WriteAllTextAsync(ConfigPath, json, Encoding.UTF8);
                await LoggingService.LogAsync("Bot", LogSeverity.Error, "No Config file found. A new one has been generated. Please close the application and fill in the required sections.");
                return;
            }

            string configFileContent = await File.ReadAllTextAsync(ConfigPath, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(configFileContent))
            {
                await LoggingService.LogAsync("Bot", LogSeverity.Error, "Config file is empty. Please fill in the required sections.");
                return;
            }

            try
            {
                Config = JsonConvert.DeserializeObject<BotConfig>(configFileContent);

                // Log the configuration values
                
                await LoggingService.LogAsync("Bot", LogSeverity.Info, $"DefaultPrefix: {Config.DefaultPrefix}");
                await LoggingService.LogAsync("Bot", LogSeverity.Info, $"GameStatus: {Config.GameStatus}");
                await LoggingService.LogAsync("Bot", LogSeverity.Info, $"BlacklistedChannels: {string.Join(", ", Config.BlacklistedChannels)}");
            }
            catch (JsonException ex)
            {
                await LoggingService.LogAsync("Bot", LogSeverity.Error, $"Error deserializing the config file: {ex.Message}");
            }
            
        }

        private static BotConfig GenerateNewConfig() => new BotConfig
        {
            DiscordToken = "",
            DefaultPrefix = "~",
            GameStatus = "CHANGE ME IN CONFIG",
            BlacklistedChannels = new List<ulong>(),
            TestGuildId = "304119646167105546"
        };
    }
}