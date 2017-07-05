using Discord;

namespace RyaBot.Handlers
{
  class EmbedGen
  {
    public EmbedGen()
    {
    }

    public Embed Generate(string message)
    {
      Embed embed = new EmbedBuilder()
                .WithColor(new Color(9912378))
                .WithDescription(message);
      return embed;
    }
  }
}
