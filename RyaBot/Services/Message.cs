using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyaBot.Services
{
  public class Msg
  {
    DiscordSocketClient _Client;

    public Msg(DiscordSocketClient _Client)
    {
      this._Client = _Client;
    }

    public async Task SendToChannel(ulong channelID, string message)
    {
      var channel = _Client.GetChannel(channelID) as ISocketMessageChannel;
      await channel.SendMessageAsync(message);
    }
  }
}
