using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace TMPHFT
{

    class Program
    {

        static Library na = new Library();

        static void Main(string[] args)
        {

     

			Console.WriteLine("Hello World! " + na.getNumber() + " - ");

		}

		static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }


    }
}
