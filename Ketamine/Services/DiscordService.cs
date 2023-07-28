using Ayako.Handlers;
using Ayako.Modules;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;
using Victoria.Node;

namespace Ayako.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly ServiceProvider _services;
        private readonly LavaNode _lavaNode;
        private readonly MusicService _audioService;
        private readonly GlobalData _globalData;

        public DiscordService()
        {
            _services = ConfigureServices();
            var config = new DiscordSocketConfig
            {
                // Set other config options as needed
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commandHandler = _services.GetRequiredService<CommandHandler>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _globalData = _services.GetRequiredService<GlobalData>();
            _audioService = _services.GetRequiredService<MusicService>();

            SubscribeDiscordEvents();
        }

        public async Task ReadyAsync()
        {
            try
            {
                await _lavaNode.ConnectAsync();
                await _client.SetGameAsync(GlobalData.Config.GameStatus);

                var _service = _services.GetRequiredService<InteractionService>();
                await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

                

                try
                {
                    await _service.RegisterCommandsGloballyAsync();
                }
                catch (HttpException exception)
                {
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                    Console.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                await LoggingService.LogAsync(ex.Source, LogSeverity.Error, ex.Message);
            }
        }

        /* Initialize the Discord Client. */
        public async Task InitializeAsync()
        {
            await InitializeGlobalDataAsync();

            await _client.LoginAsync(TokenType.Bot, GlobalData.Config.DiscordToken);
            await _client.StartAsync();

            await _commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        /* Hook Any Client Events Up Here. */

        private void SubscribeLavaLinkEvents()
        {

        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;

            _client.InteractionCreated += async (x) =>
            {
                var _service = _services.GetRequiredService<InteractionService>();
                var _provider = _services.GetRequiredService<IServiceProvider>();

                var ctx = new SocketInteractionContext(_client, x);
                await _service.ExecuteCommandAsync(ctx, _provider);
            };
        }

        private async Task InitializeGlobalDataAsync()
        {
            await _globalData.InitializeAsync();
        }

        /* Used when the Client Fires the ReadyEvent. */

        /* Used whenever we want to log something to the Console. 
           Todo: Hook in a Custom LoggingService. */
        private Task LogAsync(LogMessage logMessage)
        {
            return LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        /* Configure our Services for Dependency Injection. */
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LavaNode>()
                .AddSingleton<InteractionService>()
                .AddLogging()
                .AddLavaNode()
                .AddSingleton<PublicModule>() // Register the PublicModule
                .AddSingleton<MusicService>()
                .AddSingleton<BotService>()
                .AddSingleton<GlobalData>()
                .BuildServiceProvider();
        }
    }
}
