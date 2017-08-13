namespace RyaBot
{
  internal class Program
  {
    private static void Main()
    {
      new RyaBot().Start().GetAwaiter().GetResult();
    }
  }
}