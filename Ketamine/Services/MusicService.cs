using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace KetamineBot.Services
{
    public class MusicService
    {
        private LavaNode _LavaNode;
        
        private DiscordSocketClient _client;
        

        public MusicService(LavaNode LavaNode, DiscordSocketClient client)
        {
            _client = client;
            _LavaNode = LavaNode;
            client.Ready += OnReadyAsync;
            _LavaNode.OnLog += LogAsync;
            _LavaNode.OnTrackEnded += TrackEndedAsync;
        }
        public async Task OnReadyAsync() => await _LavaNode.ConnectAsync();

        public Task InitializeAsync()
        {

            
            return Task.CompletedTask;
        }


        private async Task TrackEndedAsync(TrackEndedEventArgs trackEndArgs)
        {
            if (!trackEndArgs.Reason.ShouldPlayNext())
                return;

            if (!trackEndArgs.Player.Queue.TryDequeue(out var track))
            {
                await trackEndArgs.Player.TextChannel.SendMessageAsync("Playback Finised");
                return;
            }


            var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Music Service, Now Playing")
                .WithDescription(
                    $"Title: {track.Title}\n" +
                    $"Author: {track.Author}\n" +
                    $"Duration: {track.Duration.ToString("h'h 'm'm 's's'")}\n\n" +
                    $"Url: [Youtube]({track.Url})")
                .WithThumbnailUrl($"https://img.youtube.com/vi/{track.Id}/maxresdefault.jpg");

            await trackEndArgs.Player.PlayAsync(track);
            await trackEndArgs.Player.TextChannel.SendMessageAsync(embed: embed.Build());
        }



        public async Task InitializeAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await InitializeAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await LeaveAsync(voiceChannel);

        public async Task<string> PlayAsync(string query, IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            var results = await _LavaNode.SearchYouTubeAsync(query);
        

            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return "No matches found.";
            }

            var track = results.Tracks.FirstOrDefault();

            if (_player.PlayerState == PlayerState.Playing)
            {
                _player.Queue.Enqueue(track);
                return $"{track.Title} has been added to the queue.";
            }
            else
            {
                await _player.PlayAsync(track);
                return $"Now Playing: {track.Title}";
            }
        }




      

        private Task ReplyAsync(Embed embed)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StopAsync(IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            if (_player is null)
                return "Error with Player";
            await _player.StopAsync();
            return "Music Playback Stopped.";
        }

        public async Task<Embed> ListAsync(IGuild guild)
        {
            try
            {
                /* Create a string builder we can use to format how we want our list to be displayed. */
                var descriptionBuilder = new StringBuilder();

                /* Get The Player and make sure it isn't null. */
                var player = _LavaNode.GetPlayer(guild);
                if (player == null)
                    return await CommandHandler.CreateErrorEmbed("Music, List", $"Could not aquire player.\nAre you using the bot right now?");

                if (player.PlayerState == PlayerState.Playing)
                {
                    /*If the queue count is less than 1 and the current track IS NOT null then we wont have a list to reply with.
                        In this situation we simply return an embed that displays the current track instead. */
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        return await CommandHandler.CreateBasicEmbed($"Now Playing: {player.Track.Title}", "Nothing Else Is Queued.", Color.Blue);
                    }
                    else
                    {
                        /* Now we know if we have something in the queue worth replying with, so we itterate through all the Tracks in the queue.
                         *  Next Add the Track title and the url however make use of Discords Markdown feature to display everything neatly.
                            This trackNum variable is used to display the number in which the song is in place. (Start at 2 because we're including the current song.*/
                        var trackNum = 2;
                        foreach (var track in player.Queue.Items)
                        {
                            descriptionBuilder.Append($"{trackNum}: [{track.Title}]({track.Url})\n");
                            trackNum++;
                        }
                        return await CommandHandler.CreateBasicEmbed("Music Playlist", $"Now Playing: [{player.Track.Title}]({player.Track.Url})\n{descriptionBuilder.ToString()}", Color.Blue);
                    }
                }
                else
                {
                    return await CommandHandler.CreateErrorEmbed("Music, List", "Player doesn't seem to be playing anything right now. If this is an error, Please Contact Draxis.");
                }
            }
            catch (Exception ex)
            {
                return await CommandHandler.CreateErrorEmbed("Music, List", ex.Message);
            }

        }





        public async Task<string> SkipAsync(IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            if (_player is null || _player.Queue.Items.Count() is 0)
                return "Nothing in queue.";

            var oldTrack = _player.Track;
            await _player.SkipAsync();
            return $"Skiped: {oldTrack.Title} \nNow Playing: {_player.Track.Title}";
        }

        public async Task<string> SetVolumeAsync(int vol, IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            if (_player is null)
                return "Player isn't playing.";

            if (vol > 150 || vol <= 2)
            {
                return "Please use a number between 2 - 150";
            }

            await _player.UpdateVolumeAsync((ushort)vol);
            return $"Volume set to: {vol}";
        }

        public async Task<string> PauseOrResumeAsync(IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            if (_player is null)
                return "Player isn't playing.";

            if (_player.PlayerState == PlayerState.Paused)
                {
                await _player.PauseAsync();
                return "Player is Paused.";
            }
            else
            {
                await _player.ResumeAsync();
                return "Playback resumed.";
            }
        }

        public async Task<string> ResumeAsync(IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            if (_player is null)
                return "Player isn't playing.";

            if (_player.PlayerState == PlayerState.Paused)
            {
                await _player.ResumeAsync();
                return "Playback resumed.";
            }

            return "Player is not paused.";
        }

      


        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
