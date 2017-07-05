using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RyaBot.Handlers;
using RyaBot.Processes;
using RyaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RyaBot
{
  public class Commands
  {
    DiscordSocketClient _Client;
    // Commands Service holding all commands
    private readonly CommandService _Commands = new CommandService();
    // Dependency Injection
    private readonly IServiceCollection _Map = new ServiceCollection();
    private IServiceProvider _Services;
    // Services
    private Settings _Settings = new Settings();
    private Msg _Message;
    private Media _Media;


    public Commands(DiscordSocketClient _Client)
    {
      this._Client = _Client;
    }

    public async Task InstallCommands()
    {
      _Message = new Msg(_Client);
      _Media = new Media(_Settings, _Message);

      _Map.AddSingleton(_Settings);
      _Map.AddSingleton(_Message);
      _Map.AddSingleton(_Media);
      _Map.AddSingleton(new Youtube());

      _Services = _Map.BuildServiceProvider();

      await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());

      _Client.MessageReceived += CmdHandler;
    }

    private async Task CmdHandler(SocketMessage arg)
    {
      var msg = arg as SocketUserMessage;
      if (msg == null || msg.Author.IsBot) return;

      int pos = 0;
      if (msg.HasCharPrefix('!', ref pos))
      {
        var context = new SocketCommandContext(_Client, msg);

        var result = await _Commands.ExecuteAsync(context, pos, _Services);

        #if DEBUG
          if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
        #endif
      }
    }
  }
}
