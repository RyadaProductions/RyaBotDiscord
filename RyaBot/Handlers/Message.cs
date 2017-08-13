using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace RyaBot.Handlers
{
  public class Message
  {
    private readonly DiscordSocketClient _client;

    public Message(DiscordSocketClient client)
    {
      _client = client ?? throw new ArgumentNullException(nameof(DiscordSocketClient));
    }

    public async Task SendToChannel(ulong channelId, string message)
    {
      var channel = _client.GetChannel(channelId) as ISocketMessageChannel;
      if (channel != null) await channel.SendMessageAsync(message);
    }
  }
}