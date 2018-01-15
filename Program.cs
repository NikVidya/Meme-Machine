using System;
using RedditSharp;
using Discord;
using System.Threading.Tasks;

namespace memeMachine
{
    class Program
    {
        private Discord.WebSocket.DiscordSocketClient _client;
        private ulong dankmachine = 402382830589444097;
        
        private Reddit reddit;

        public static void Main(string[] args)
             => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.WriteLine("Hello World!");
            _client = new Discord.WebSocket.DiscordSocketClient();
            string token = "***REMOVED***";
            _client.Ready += ScrapeReddit;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task ScrapeReddit() {
            reddit = new Reddit();
            var channel = _client.GetChannel(dankmachine)as Discord.WebSocket.SocketTextChannel;
            if(channel == null) {
                await Log("Could not find memeMachine");
                return;
            }
            var subreddit = reddit.GetSubreddit("/r/youtubehaiku");
            foreach (var post in subreddit.Hot.GetListing(5))
            {
                if (channel.Name.Equals("dankmachine"))
                {
                    await channel.SendMessageAsync(post.Url.ToString());
                }
                else
                {
                    await Log(channel.Name);
                }
                await Log(post.Url.ToString());
            }
            await _client.LogoutAsync();
        }

        private Task Log(String msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
