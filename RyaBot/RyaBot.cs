using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RyaBot.Handlers;
using RyaBot.Processes;
using RyaBot.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RyaBot
{
  class RyaBot
  {
    private DiscordSocketClient _Client;
    private readonly IServiceCollection _Map = new ServiceCollection();
    private readonly CommandService _Commands = new CommandService();
    private Settings _Settings = new Settings();
    private Msg _Message;
    private Media _Media;

    public RyaBot()
    {
      Console.Title = "RyaBot";

      _Client = new DiscordSocketClient(new DiscordSocketConfig {
        LogLevel = LogSeverity.Info,
      });
    }

    #region MainAsync
    public async Task Start()
    {

      string token = Environment.GetEnvironmentVariable("token");
      _Client.Log += Logger;

      await InitCommands();

      await _Client.LoginAsync(TokenType.Bot, token);
      await _Client.StartAsync();

      await Task.Delay(-1);
    }
    #endregion

    private IServiceProvider _Services;

    #region InitCommands
    private async Task InitCommands()
    {
      _Message = new Msg(_Client);
      _Media = new Media(_Settings, _Message);

      _Map.AddSingleton(_Settings);
      _Map.AddSingleton(_Message);
      _Map.AddSingleton(_Media);
      _Map.AddSingleton(new Youtube());

      _Services = _Map.BuildServiceProvider();

      //await _Commands.AddModulesAsync(new Voice());
      await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());

      _Client.MessageReceived += CmdHandler;
    }
    #endregion

    #region CmdHandler
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
        if (!result.IsSuccess)
          await context.Channel.SendMessageAsync(result.ErrorReason);
#endif
      }
    }
    #endregion

    #region Logger
    private static Task Logger(LogMessage message)
    {
      var cc = Console.ForegroundColor;
      switch (message.Severity)
      {
        case LogSeverity.Critical:
        case LogSeverity.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case LogSeverity.Warning:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
        case LogSeverity.Info:
          Console.ForegroundColor = ConsoleColor.White;
          break;
        case LogSeverity.Verbose:
        case LogSeverity.Debug:
          Console.ForegroundColor = ConsoleColor.DarkGray;
          break;
      }
      Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
      Console.ForegroundColor = cc;
      return Task.CompletedTask;
    }
    #endregion
  }
}
