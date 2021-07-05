using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMPFT.Screen;

namespace TMPFT
{

    class Program
    {

        //static Library na = new Library();
        static StateMachine screen = new StateMachine();

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
