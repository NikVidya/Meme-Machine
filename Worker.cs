using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace MemeMachine
{
    class Worker : IHostedService
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        public Task StartAsync(CancellationToken stopToken)
        {
            new Worker().MainAsync().GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stopToken)
        {
            _client.LogoutAsync();
            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            string token = Program.Token;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await InstallCommandsAsync();
            await Log("Logged in");
        }

        private async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            var msg = socketMessage as SocketUserMessage;
            if (msg == null) return;
            if (msg.Author.IsBot) return;

            SocketCommandContext context = new SocketCommandContext(_client, msg);
            await _commandService.ExecuteAsync(context: context, 0, services: null);
        }

        private Task Log(String msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}