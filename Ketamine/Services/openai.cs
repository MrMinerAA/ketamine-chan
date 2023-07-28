/*
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenAI_API;
using System;
using System.Reflection;
using System.Threading.Tasks;

public class ChatGptBot
{
    private OpenAIAPI openAiApi;

    public ChatGptBot(string apiKey)
    {
        openAiApi = new OpenAIAPI("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");
    }

    public async Task<string> GetChatGptResponse(string message)
    {
        var prompt = $"You are a helpful assistant that answers questions. Ask me anything!\n\nQ: {message}\nA:";
        var completions = await openAiApi.CompleteConversationAsync(prompt, model: "text-davinci-003", temperature: 0.7, maxTokens: 150);
        var response = completions.Choices[0].Text.Trim();

        return response;
    }


    public class MyBot
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private ChatGptBot _chatGptBot;

        public MyBot(ChatGptBot chatGptBot)
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _chatGptBot = chatGptBot;
        }

        public async Task RunBotAsync()
        {
            await _client.LoginAsync(TokenType.Bot, "YOUR_DISCORD_TOKEN");
            await _client.StartAsync();

            await _client.SetGameAsync("ChatGpt Bot is online!");

            _client.Log += Log;

            await RegisterCommandsAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            // Define the command prefix character (e.g., "!").
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot) return;

            if (message.Content.StartsWith("!"))
            {
                var commandContext = new CommandContext(_client, message);
                var commandServiceResult = await _commands.ExecuteAsync(commandContext, 1, null);

                if (!commandServiceResult.IsSuccess)
                {
                    await commandContext.Channel.SendMessageAsync($"Error: {commandServiceResult.ErrorReason}");
                }
            }
            else
            {
                // Send the user message to ChatGPT and get the response
                var response = await _chatGptBot.GetChatGptResponse(message.Content);

                // Send the response back to the Discord channel
                await context.Channel.SendMessageAsync(response);
            }
        }
    }



    public class Program
    {
        static void Main(string[] args)
        {
            var chatGptBot = new ChatGptBot("YOUR_API_KEY");
            var bot = new MyBot(chatGptBot);

            bot.RunBotAsync().GetAwaiter().GetResult();
        }
    }

}
*/
