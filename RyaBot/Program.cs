namespace RyaBot
{
  class Program
  {
    static void Main(string[] args)
    {
      new RyaBot().Start().GetAwaiter().GetResult();
    }
  }
}
