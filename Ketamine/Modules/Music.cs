﻿using Ayako.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Node;
using Victoria.Player;
using Victoria.Resolvers;
using Victoria.Responses.Search;

namespace Ayako.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);
        private readonly LavaNode _lavaNode;
        private readonly MusicService _musicService;

        public MusicService AudioService { get; set; }
        public static List<Embed> embeds { get; set; }


        public Music(LavaNode lavaNode, MusicService musicService)
        {
            _lavaNode = lavaNode;
            _musicService = musicService;
        }


        [SlashCommand("join", "Join a voice channel")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("leave", "Leave the voice channel")]
        public async Task LeaveAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to any voice channels!");
                return;
            }

            var voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null)
            {
                await ReplyAsync("Not sure which voice channel to disconnect from.");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                await ReplyAsync($"I've left {voiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("play", "Play a song")]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                var voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel == null)
                {
                    await ReplyAsync("You must be connected to a voice channel!");
                    return;
                }

                try
                {
                    player = await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }

            var searchResponse = await _lavaNode.SearchAsync(Uri.IsWellFormedUriString(searchQuery, UriKind.Absolute) ? SearchType.Direct : SearchType.YouTube, searchQuery);
            if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                player.Vueue.Enqueue(searchResponse.Tracks);
                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} songs.");
            }
            else
            {
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Vueue.Enqueue(track);

                await ReplyAsync($"Enqueued {track?.Title}");
            }

            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                return;
            }

            player.Vueue.TryDequeue(out var lavaTrack);
            await player.PlayAsync(lavaTrack);
        }

        [SlashCommand("pause", "Pause the currently playing song")]
        public async Task PauseAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("I cannot pause when I'm not playing anything!");
                return;
            }

            try
            {
                await player.PauseAsync();
                await ReplyAsync($"Paused: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("resume", "Resume the currently paused song")]
        public async Task ResumeAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Paused)
            {
                await ReplyAsync("I cannot resume when I'm not playing anything!");
                return;
            }

            try
            {
                await player.ResumeAsync();
                await ReplyAsync($"Resumed: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("stop", "Stop the currently playing song")]
        public async Task StopAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Woaaah there, I can't stop the stopped forced.");
                return;
            }

            try
            {
                await player.StopAsync();
                await ReplyAsync("No longer playing anything.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("skip", "Skip the currently playing song")]
        public async Task SkipAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I can't skip when nothing is playing.");
                return;
            }

            var voiceChannelUsers = (player.VoiceChannel.Guild as SocketGuild).Users
                .Where(x => !x.IsBot)
                .ToArray();
            if (_musicService.VoteQueue.Contains(Context.User.Id))
            {
                await ReplyAsync("You can't vote again.");
                return;
            }

            _musicService.VoteQueue.Add(Context.User.Id);
            var percentage = _musicService.VoteQueue.Count / voiceChannelUsers.Length * 100;
            if (percentage < 85)
            {
                await ReplyAsync("You need more than 85% votes to skip this song.");
                return;
            }

            try
            {
                var (skipped, currenTrack) = await player.SkipAsync();
                await ReplyAsync($"Skipped: {skipped.Title}\nNow Playing: {currenTrack.Title}");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("seek", "Seek to a specific position in the currently playing song")]
        public async Task SeekAsync(TimeSpan timeSpan)
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I can't seek when nothing is playing.");
                return;
            }

            try
            {
                await player.SeekAsync(timeSpan);
                await ReplyAsync($"I've seeked `{player.Track.Title}` to {timeSpan}.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("volume", "Set the player volume")]
        public async Task VolumeAsync(ushort volume)
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            try
            {
                await player.SetVolumeAsync(volume);
                await ReplyAsync($"I've changed the player volume to {volume}.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("nowplaying", "Show the currently playing song")]
        public async Task NowPlayingAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var track = player.Track;
            var artwork = await track.FetchArtworkAsync();

            var embed = new EmbedBuilder()
                .WithAuthor(track.Author, Context.Client.CurrentUser.GetAvatarUrl(), track.Url)
                .WithTitle($"Now Playing: {track.Title}")
                .WithImageUrl(artwork)
                .WithFooter($"{track.Position}/{track.Duration}");

            await ReplyAsync(embed: embed.Build());
        }

        [SlashCommand("genius", "Show the Genius lyrics of the currently playing song")]
        public async Task ShowGeniusLyrics()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var lyrics = await LyricsResolver.SearchGeniusAsync(player.Track);
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync($"No lyrics found for {player.Track.Title}");
                return;
            }

            var splitLyrics = lyrics.Split(Environment.NewLine);
            var stringBuilder = new StringBuilder();
            foreach (var line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line.TrimEnd('\n'));
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }

        [SlashCommand("ovh", "Show the OVH lyrics of the currently playing song")]
        public async Task ShowOvhLyrics()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var lyrics = await LyricsResolver.SearchOvhAsync(player.Track);
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync($"No lyrics found for {player.Track.Title}");
                return;
            }

            var splitLyrics = lyrics.Split(Environment.NewLine);
            var stringBuilder = new StringBuilder();
            foreach (var line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }


    }
}
