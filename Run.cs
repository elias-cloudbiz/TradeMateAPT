using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMPFT.Core;
using TMPFT.Display;

namespace TMPFT
{

    class Run
    {

        //static Library na = new Library();
        static RunStateMachine StateMachineView = new RunStateMachine();


        static void Main(string[] args)
        {

            StateMachineView.Start(args);

            //CoreUI.Start();
        }

        static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }


    }
}
