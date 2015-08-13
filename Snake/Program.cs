using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            using (var game = new GameWindow())
                game.Run(15, 30);

            Console.WriteLine("Exiting");
        }
    }
}
