using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMPHFT.Screen;

namespace TMPHFT
{

    class Program
    {

        static Library na = new Library();
        static ViewScreen2 screen = new ViewScreen2();

        static void Main(string[] args)
        {

            screen.Start(args);
		}

		static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }


    }
}
