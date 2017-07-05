using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
