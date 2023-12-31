﻿using System.Collections.Generic;

namespace Ayako.DataStructs
{
    public class BotConfig
    {
        public string DiscordToken { get; set; }
        public string DefaultPrefix { get; set; }
        public string GameStatus { get; set; }
        public string TestGuildId { get; set; }
        public List<ulong> BlacklistedChannels { get; set; }
    }
} 