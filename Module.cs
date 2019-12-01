using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.Commands;
using RedditSharp;
namespace MemeMachine
{
    public class Module : ModuleBase<SocketCommandContext>
    {
        private string _subredditName = "";
        private ulong _channelId;
        private Reddit _reddit;
        [Command("give me memes")]
        public async Task ScrapeReddit()
        {
            _subredditName = Program.ProjectConfiguration["subreddit"];
            _channelId = UInt64.Parse(Program.ProjectConfiguration["channelId"]);
            _reddit = new Reddit();
            var channel = Context.Client.GetChannel(_channelId) as Discord.WebSocket.SocketTextChannel;
            await Log("Got Channel");
            if (channel == null)
            {
                await Log("Could not find memeMachine");
                return;
            }
            var subreddit = _reddit.GetSubreddit("/r/youtubehaiku");
            await Log("Created subreddit var");
            switch (_subredditName)
            {
                case "":
                    subreddit = _reddit.GetSubreddit("/r/youtubehaiku");
                    break;
                default:
                    subreddit = _reddit.GetSubreddit("/r/" + _subredditName);
                    break;
            }
            await Log("Got subreddit");
            var urlList = new List<string>();
            foreach (var post in subreddit.Hot.GetListing(5))
            {
                if (notASelfPost(post.Url.ToString()))
                {
                    urlList.Add(post.Url.ToString());
                }
                else
                {
                    await Log(channel.Name);
                }
                await Log(post.Url.ToString());
            }
            string s = string.Join(" ", urlList.ToArray());
            await channel.SendMessageAsync("RETRIEVING MEMES FROM /r/" + _subredditName + "... DONE\n " + s);
        }


        private bool notASelfPost(string matchme)
        {
            return (!matchme.Contains("redd"));
        }

        private Task Log(String msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}