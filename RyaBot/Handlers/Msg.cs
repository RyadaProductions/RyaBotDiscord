using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyaBot.Handlers
{
  public class Msg
  {
    private readonly DiscordSocketClient _client;

    public Msg(DiscordSocketClient client)
    {
      _client = client;
    }

    public async Task SendToChannel(ulong channelID, string message)
    {
      var channel = _client.GetChannel(channelID) as ISocketMessageChannel;
      await channel.SendMessageAsync(message);
    }
  }
}
