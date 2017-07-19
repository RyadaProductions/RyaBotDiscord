namespace RyaBot
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      new RyaBot().Start().GetAwaiter().GetResult();
    }
  }
}