using Discord;
using System;

namespace Ayako.Services
{
    class EmbBuild
    {
        public static Embed Build(string message, Tuple<string, Color> statusType)
        {
            var emb = new EmbedBuilder().WithDescription($"{statusType.Item1} {message}").WithColor(statusType.Item2);
            return emb.Build();
        }
    }

    class Status
    {
        public static Tuple<string, Color> Success()
        {
            string statusEmote = ":white_check_mark:";
            Color color = new Color(0, 165, 2);
            return Tuple.Create(statusEmote, color);
        }

        public static Tuple<string, Color> Failure()
        {
            string statusEmote = ":x:";
            Color color = new Color(216, 65, 65);
            return Tuple.Create(statusEmote, color);
        }
    }
}
