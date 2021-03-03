using System;
using TMPHFT;

namespace TradeMateHFT
{
    class Program
    {

        static Library na = new Library();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! " + na.getNumber());
        }
    }
}
