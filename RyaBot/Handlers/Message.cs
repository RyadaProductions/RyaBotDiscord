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

    public async Task SendToChannel(ulong channelID, string message)
    {
      var channel = _client.GetChannel(channelID) as ISocketMessageChannel;
      await channel.SendMessageAsync(message);
    }
  }
}