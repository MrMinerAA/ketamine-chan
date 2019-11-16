using Discord;
using Discord.Commands;
using Discord.WebSocket;
using KetamineBot.Services;
using System.Threading.Tasks;
using Victoria;
using System;


namespace KetamineBot.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        private readonly MusicService _musicService;

        public Music(LavaNode lavaNode, MusicService musicService)
        {
            _lavaNode = lavaNode;
            _musicService = musicService;
        }

       

        [Command("Join")]
        [Alias("J")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You need to connect to a voice channel.");
                return;
            }
            else
            {
                await _lavaNode.JoinAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"now connected to {user.VoiceChannel.Name}");
            }
        }


        [Command("Move")]
        [Alias("M")]
        public async Task MoveAsync()
        {
            var user = Context.User as SocketGuildUser;
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("Please join the channel the bot is in to make it leave.");
            }
            else
            {
                await _lavaNode.MoveAsync(user.VoiceChannel);
                await ReplyAsync($"Moved from {player.VoiceChannel} {user.VoiceChannel.Name}");
            }
        }

        [Command("Leave")]
        [Alias("L")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("Please join the channel the bot is in to make it leave.");
            }
            else
            {
                await _lavaNode.LeaveAsync(user.VoiceChannel);
                await ReplyAsync($"Bot has now left {user.VoiceChannel.Name}");
            }
        }



        [Command("Play")]
        [Alias("P")]
        public async Task Play([Remainder]string query)
            => await ReplyAsync(await _musicService.PlayAsync(query, Context.Guild));

        
        [Command("Queue")]
        [Alias("q")]
        public async Task List()
            => await ReplyAsync("", false, await _musicService.ListAsync(Context.Guild));
            

        [Command("Stop")]
        [Alias("S")]
        public async Task Stop()
            => await ReplyAsync(await _musicService.StopAsync(Context.Guild));

        [Command("Skip")]
        [Alias("Sk")]
        public async Task Skip()
            => await ReplyAsync(await _musicService.SkipAsync(Context.Guild));

        [Command("Volume")]
        [Alias("V")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol, Context.Guild));

        [Command("Pause")]
        [Alias("Pa")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync(Context.Guild));

        [Command("Resume")]
        [Alias("Re")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync(Context.Guild));





    }
}
