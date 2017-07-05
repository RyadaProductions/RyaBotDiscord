using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RyaBot.Main;

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
