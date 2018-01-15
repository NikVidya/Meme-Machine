using System;
using RedditSharp;
using Discord;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace memeMachine
{
    class Program
    {
        private Discord.WebSocket.DiscordSocketClient _client;
        private ulong dankmachine = 402382830589444097;
        
        private Reddit reddit;

        public static void Main(string[] args)
             => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            _client = new Discord.WebSocket.DiscordSocketClient();
            string token = args[0];
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
                if (channel.Name.Equals("dankmachine") && isItAYoutube(post.Url.ToString()))
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
           private bool isItAYoutube(string matchme)
        {
            string pattern = "#^(?:https?://)?";    // Optional Url scheme. Either http or https
            pattern += "(?:www\\.)?";               // Optional www subdomain
            pattern += "(?:";                       // Hosts group:
            pattern += "youtu\\.be/";               //   Either youtu.be, 
            pattern += "|youtube\\.com";            //   or youtube.com 
            pattern += "(?:";                       //   Paths group: 
            pattern += "/embed/";                   //     Either /embed/, 
            pattern += "|/v/";                      //	   or /v/,
            pattern += "|/watch\\?v=";              //     or /watch?v=, 
            pattern += "|/watch\\?.+&v=";           //     or /watch?other_param&v= 
            pattern += ")";                         //   End paths group. 
            pattern += ")";                         // End hosts group.
            pattern += "([\\w-]{11})";              // 11 characters (Length of Youtube video ids).
            pattern += "(?:.+)?$#x";	            // Optional other ending URL parameters.
            var regex = new Regex(pattern);
            return regex.IsMatch(matchme);
        }
    }
}
