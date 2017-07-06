using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RyaBot.Handlers;
using RyaBot.Main;
using RyaBot.Models;
using RyaBot.Processes;
using RyaBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RyaBot
{
  public class RyaBot
  {
    private readonly DiscordSocketClient _client;

    public RyaBot()
    {
      Console.Title = "RyaBot";

      _client = new DiscordSocketClient(new DiscordSocketConfig {
        LogLevel = LogSeverity.Info,
      });
    }

    public async Task Start()
    {

      var token = Environment.GetEnvironmentVariable("token");
      _client.Log += Logger;

      await new Installer(_client).InstallCommands();

      await _client.LoginAsync(TokenType.Bot, token);
      await _client.StartAsync();

      await Task.Delay(-1);
    }

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
  }
}
