using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Ketamine.Services;
using System.IO;
using KetamineBot;

namespace Ketamine
{
    // This is a minimal example of using Discord.Net's command
    // framework - by no means does it show everything the framework
    // is capable of.
    //
    // You can find samples of using the command framework:
    // - Here, under the 02_commands_framework sample
    // - https://github.com/foxbot/DiscordBotBase - a bare-bones bot template
    // - https://github.com/foxbot/patek - a more feature-filled bot, utilizing more aspects of the library
    class Program
    {
        // There is no need to implement IDisposable like before as we are
        // using dependency injection, which handles calling Dispose for us.
        static async Task Main(string[] args)
         => await new KetamineClient().InitializeAsync();



        public async Task Ketamine(SocketMessage msg)
        {
            if (msg.Author.IsBot)
                return;
            if (msg.Author.Equals("258938877077618690"))
                return;
            if (msg.Content.StartsWith('~'))
                return;
            if (msg.Content.StartsWith('.'))
                return;
            var usrs = msg.Channel.GetUsersAsync();

            IUser cbt = await msg.Channel.GetUserAsync(134127979373658112);
            string cbtName = cbt.Username;

            Color successColor = new Color(uint.Parse("34eb7d", System.Globalization.NumberStyles.HexNumber));
            Embed msgEmbed = new EmbedBuilder()
                .WithDescription($"Gold BackPack-Kun & {cbtName}")
                .WithImageUrl("https://i.imgur.com/BiEEMMp.jpg")
                .WithUrl("https://www.webmd.com/depression/features/what-does-ketamine-do-your-brain#1").Build();
            await msg.Channel.SendMessageAsync("", embed: msgEmbed);
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                
                .BuildServiceProvider();
        }
    }
}
