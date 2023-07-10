using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Ayako.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public static string path = System.IO.Directory.GetCurrentDirectory();

        [Command("random")]
        public async Task RandomCommand()
        {
            string[] responses =
{
                "Ha 69, Thats The Sex Number",
                "Ayako",
                "Black Tar Heroine",
                "h-hewwo...owunce of weed pwease >///< arigato... dealer-kun puts weedie-chan in bong and inhales waaah!! (╯✧▽✧)╯ daisuki cannabis desu~! (^ ω ^ )uwaaa! weedie-chan i feel so kimochi!!(〃°ω°〃) hehe~ ur deep inside me now x3 weedie-chans drug pussy is so good!",
                "Rawr X3 *nuzzles* How are you? *pounces on you* you're so warm o3o *notices you have a bulge* someone's happy! *nuzzles your necky wecky* ~murr~ hehe ;) *rubbies your bulgy wolgy* you're so big! *rubbies more on your bulgy wolgy* it doesn't stop growing .///. *kisses you and licks your neck* daddy likes ;) *nuzzle wuzzle* I hope daddy likes *wiggles butt and squirms* I wanna see your big daddy meat! *wiggles butt* I have a little itch o3o *wags tails* can you please get my itch? *put paws on your chest* nyea~ it's a seven inch itch *rubs your chest* can you pwease? *squirms* pwetty pwease? :( I need to be punished *runs paws down your chest and bites lip* like, I need to be punished really good *paws on your bulge as I lick my lips* I'm getting thirsty. I could go for some milk *unbuttons your pants as my eyes glow* you smell so musky ;) *licks shaft* mmmmmmmmmmmmmmmmmmm so musky ;) *drools all over your cawk* your daddy meat. I like. Mister fuzzy balls. *puts snout on balls and inhales deeply* oh my gawd. I'm so hard *rubbies your bulgy wolgy* *licks balls* punish me daddy nyea~ *squirms more and wiggles butt* I9/11 lovewas an yourinside muskyjob goodness *bites lip* please punish me *licks lips* nyea~ *suckles on your tip* so good *licks pre off your cock* salty goodness~ *eyes roll back and goes balls deep*",
                "UwU This is the LA Powice Depawtment, we awe fowwowing up on a sewies of weports of that youw huge bolgy wolgy has been distuwbing the peace in the neighbowhood, so we will be taking you and youw bolgy wolgy OwO into ouw custody, and it is pawt of the pwoceduwe fow us to nuzzle and wuzzle youw necky wecky ~ murr~ hehe UwU"
            };

            Random rnd = new Random();
            int responseIndex = rnd.Next(0, 6);

            await Context.Channel.SendMessageAsync($"{responses[responseIndex]}");
        }

        [Command("uwu")]
        public async Task UwU()
        {
            string str = "Ketamine is a medication mainly used for starting and maintaining anesthesia. It induces a trance-like state while providing pain relief, sedation, and memory loss. Other uses include for chronic pain, sedation in intensive care, and depression. Heart function, breathing, and airway reflexes generally remain functional. Effects typically begin within five minutes when given by injection, and last up to about 25 minutes. Common side effects include agitation, confusion, or hallucinations as the medication wears off. Elevated blood pressure and muscle tremors are relatively common. Spasms of the larynx may rarely occur. Ayako is an NMDA receptor antagonist, but it may also have other actions. Ayako was discovered in 1962, first tested in humans in 1964, and was approved for use in the United States in 1970. It was extensively used for surgical anesthesia in the Vietnam War due to its safety. It is on the World Health Organization's List of Essential Medicines, the most effective and safe medicines needed in a health system.[28] It is available as a generic medication. The wholesale price in the developing world is between US$0.84 and US$3.22 per vial. Ayako is also used as a recreational drug for its hallucinogenic and dissociative effects.";

            Color successColor = new Color(uint.Parse("34eb7d", System.Globalization.NumberStyles.HexNumber));
            Embed msgEmbed = new EmbedBuilder()
                .WithImageUrl("https://drugabuse.com/wp-content/uploads/drugabuse_shutterstock-153900788-young-girls-snorting-Ayako-off-mirror-with-marked-bills.jpg")
                .WithTitle("Ketamine").Build();


            await Context.Channel.SendMessageAsync("", embed: msgEmbed);
        }

        [Command("h")]
        [Alias("help")]
        public async Task Help()
        {

            Embed msgEmbed = new EmbedBuilder()
                .WithTitle("Help")
                .WithDescription("Play, Pause, Resume, Stop, Queue, and Leave are a few of my music based commands.")
                .WithColor(Color.DarkPurple)
                .WithCurrentTimestamp().Build();


            await Context.Message.Author.SendMessageAsync("", embed: msgEmbed);
        }

        [Command("Crazy")]
        public async Task Crazy()
        {
            string str = "Crazy ? I was crazy once. They locked me in a room. A rubber room.A rubber room with rats.And rats make me crazy.";

            await Context.Channel.SendMessageAsync("");
        }


    }
}


/*
 *          Color successColor = new Color(uint.Parse("34eb7d", System.Globalization.NumberStyles.HexNumber));
            Embed msgEmbed = new EmbedBuilder()
                .WithDescription(str)
                .WithFooter("footer")
                .WithColor(successColor)
                .WithImageUrl("https://link.com/")
                .WithUrl("https://link.com/")
                .WithTimestamp(DateTime.Now)
                .WithThumbnailUrl("https://link.com/")
                .WithTitle("Title").Build();
 */
