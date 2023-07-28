using Ayako.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ayako.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            HookEvents();
        }

        public async Task InitializeAsync()
        {
            // Register the type reader for CommandContext
            _commands.AddTypeReader<CommandContext>(new CommandContextTypeReader());

            // Register the PublicModule
            await _commands.AddModuleAsync<PublicModule>(_services);

            // Add modules from the entry assembly
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public void HookEvents()
        {
            _commands.CommandExecuted += CommandExecutedAsync;
            _commands.Log += LogAsync;
            _client.MessageReceived += HandleCommandAsync;
            
        }

        public async Task RegisterSlashCommandsAsync()
        {
            var guilds = _client.Guilds;
            int delayBetweenCommandsMs = 500; // Adjust this value as needed (in milliseconds)

            foreach (var guild in guilds)
            {
                foreach (var module in _commands.Modules)
                {
                    foreach (var command in module.Commands)
                    {
                        var slashCommand = new SlashCommandBuilder()
                            .WithName(command.Name)
                            .WithDescription(command.Summary);

                        // Add options to the slash command if needed
                        // slashCommand.AddOption(...);

                        var createdCommand = await _client.Rest.CreateGuildCommand(slashCommand.Build(), guild.Id);

                        // Logging the command information
                        if (createdCommand != null)
                        {
                            Console.WriteLine($"Registered slash command '{createdCommand.Name}' in guild '{guild.Name}'");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to register slash command '{command.Name}' in guild '{guild.Name}'");
                        }

                        await Task.Delay(delayBetweenCommandsMs);
                    }
                }
            }
        }


        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            Console.WriteLine("HandleCommandAsync called");

            var argPos = 0;
            if (!(socketMessage is SocketUserMessage message) || message.Author.IsBot || message.Author.IsWebhook )
                return;

            var context = new SocketCommandContext(_client, message);

            var blacklistedChannelCheck = from a in GlobalData.Config.BlacklistedChannels
                                          where a == context.Channel.Id
                                          select a;
            var blacklistedChannel = blacklistedChannelCheck.FirstOrDefault();

            if (blacklistedChannel == context.Channel.Id)
            {
                return;
            }
            else
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    // Log the error to the console
                    Console.WriteLine($"Command execution failed: {result.ErrorReason}");

                    // Send an error message to the channel where the command was invoked
                    var errorMessage = $"Command execution failed: {result.ErrorReason}";
                    var embed = new EmbedBuilder()
                        .WithTitle("Command Execution Failed")
                        .WithDescription(errorMessage)
                        .WithColor(Color.Red)
                        .Build();
                    
                }
            }
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
                return;

            await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp().Build());
            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string source, string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($"ERROR OCCURRED FROM - {source}")
                .WithDescription($"**Error Details**: \n{error}")
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp().Build());
            return embed;
        }
    }
}
