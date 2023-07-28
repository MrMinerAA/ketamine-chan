using Discord;
using System;
using System.Threading.Tasks;

namespace Ayako.Services
{
    /* A Static Logging Service So it Can Be Used Throughout The Whole Bot Anywhere We Want. */
    public static class LoggingService
    {
        /* The Standard Way Log */
        public static async Task LogAsync(string src, LogSeverity severity, string message, Exception exception = null)
        {
            if (severity == LogSeverity.Debug)
                return;

            if (severity == LogSeverity.Error && exception != null)
            {
                message += $"\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            }

            await Append($"[{DateTime.Now:HH:mm:ss}] [{GetSeverityString(severity)}] [{SourceToString(src)}] {message}\n", GetConsoleColor(severity));
        }

        /* The Way To Log Critical Errors*/
        public static async Task LogCriticalAsync(string source, string message, Exception exc = null)
            => await LogAsync(source, LogSeverity.Critical, message, exc);

        /* The Way To Log Basic Infomation */
        public static async Task LogInformationAsync(string source, string message)
            => await LogAsync(source, LogSeverity.Info, message);

        /* Format The Output */
        private static async Task Append(string message, ConsoleColor color)
        {
            await Task.Run(() =>
            {
                Console.ForegroundColor = color;
                Console.Write(message);
            });
        }

        /* Swap The Normal Source Input To Something Neater */
        private static string SourceToString(string src)
        {
            switch (src.ToLower())
            {
                case "discord":
                    return "DISC";
                case "victoria":
                    return "VICT";
                case "audio":
                    return "AUDI";
                case "admin":
                    return "ADMN";
                case "gateway":
                    return "GATE";
                case "blacklist":
                    return "BLCK";
                case "lavanode_0_socket":
                    return "LAVA";
                case "lavanode_0":
                    return "LAVA";
                case "bot":
                    return "BOT";
                default:
                    return src;
            }
        }

        /* Swap The Severity To a String So We Can Output It To The Console */
        private static string GetSeverityString(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return "CRIT";
                case LogSeverity.Debug:
                    return "DBUG";
                case LogSeverity.Error:
                    return "ERRO";
                case LogSeverity.Info:
                    return "INFO";
                case LogSeverity.Verbose:
                    return "VRBS";
                case LogSeverity.Warning:
                    return "WARN";
                default:
                    return "UNKN";
            }
        }

        /* Return The Console Color Based On Severity Selected */
        private static ConsoleColor GetConsoleColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return ConsoleColor.Red;
                case LogSeverity.Debug:
                    return ConsoleColor.Cyan;
                case LogSeverity.Error:
                    return ConsoleColor.DarkRed;
                case LogSeverity.Info:
                    return ConsoleColor.Green;
                case LogSeverity.Verbose:
                    return ConsoleColor.Gray;
                case LogSeverity.Warning:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
