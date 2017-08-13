using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RyaBot.Handlers;
using RyaBot.Processes;
using System;
using System.Reflection;
using System.Threading.Tasks;
using RyaBot.Models;

namespace RyaBot.Main
{
  public class Installer
  {
    private readonly DiscordSocketClient _client;

    // Commands Service holding all commands
    private readonly CommandService _commands;

    // Dependency Injection
    private IServiceProvider _services;

    // Services
    private readonly Settings _settings;

    private readonly Message _message;
    private readonly Media _media;

    public Installer(DiscordSocketClient client)
    {
      _client = client ?? throw new ArgumentNullException(nameof(DiscordSocketClient));

      _commands = new CommandService();

      _settings = new Settings();
      _message = new Message(_client);
      _media = new Media(_settings, _message);
    }

    public async Task InstallCommands()
    {
      IServiceCollection map = new ServiceCollection();

      map.AddSingleton(_settings);
      map.AddSingleton(_message);
      map.AddSingleton(_media);
      map.AddSingleton(new Youtube());

      _services = map.BuildServiceProvider();

      await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

      _client.MessageReceived += CmdHandler;
    }

    private async Task CmdHandler(SocketMessage arg)
    {
      var msg = arg as SocketUserMessage;
      if (msg == null || msg.Author.IsBot) return;

      var pos = 0;
      if (msg.HasCharPrefix('!', ref pos))
      {
        var context = new SocketCommandContext(_client, msg);

        var result = await _commands.ExecuteAsync(context, pos, _services);
        
        if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
      }
    }
  }
}