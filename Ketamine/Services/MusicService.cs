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
        }
        public async Task OnReadyAsync() => await _LavaNode.ConnectAsync();

        public Task InitializeAsync()
        {

            _LavaNode.OnLog += LogAsync;
            _LavaNode.OnTrackEnded += TrackEnded;
            return Task.CompletedTask;
        }






        public async Task InitializeAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await InitializeAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await LeaveAsync(voiceChannel);

        public async Task<string> PlayAsync(string query, IGuild guild)
        {
            var _player = _LavaNode.GetPlayer(guild);
            var results = await _LavaNode.SearchYouTubeAsync(query);
        /*
            Console.WriteLine(results.Tracks.Count());
            if(results.Tracks.Count() > 1)
            {
                foreach (var _track in results.Tracks)
                {
                    _player.Queue.Enqueue(_track);
                    return $"added {results.Tracks.Count()} song(s) to the queue.";
                }
                
           }
           */

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




        private async Task TrackEnded(TrackEndedEventArgs e)
        {
            if (!e.Reason.ShouldPlayNext())
                return;

            if (!e.Player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await e.Player.TextChannel.SendMessageAsync("There are no more tracks in the queue.");
                return;
            }

            await e.Player.PlayAsync(nextTrack);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
