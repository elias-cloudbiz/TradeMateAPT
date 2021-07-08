using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMPFT.Core;
using TMPFT.Screen;

namespace TMPFT
{

    class Run
    {

        //static Library na = new Library();
        static StateMachine StateMachineView = new StateMachine();
        static CoreUI CoreUI = new CoreUI(); 

        static void Main(string[] args)
        {

            StateMachineView.Start(args);
            CoreUI.Start();
		}

		static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }


    }
}
