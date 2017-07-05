using Discord;

namespace RyaBot.Handlers
{
  public class EmbedGen
  {
    public EmbedGen()
    {
    }

    public Embed Generate(string message)
    {
      var embed = new EmbedBuilder()
                .WithColor(new Color(9912378))
                .WithDescription(message);
      return embed;
    }
  }
}
