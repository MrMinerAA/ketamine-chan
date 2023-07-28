using Ayako.Services;
using System;
using System.Threading.Tasks;

namespace Ayako
{
    class Program
    {
        internal static bool IsDebug()
        {
            throw new NotImplementedException();
        }

        static async Task Main(string[] args)
        {
            var discordService = new DiscordService();
            await discordService.InitializeAsync();
        }
    }
}