using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMAPT.Core;
using TMAPT.Display;

namespace TMAPT
{

    class Run
    {

        //static Library na = new Library();
        static StateMachine StateMachineView = new StateMachine();


        static async Task Main(string[] args)
        {

            await StateMachineView.Start(args);

            //CoreUI.Start();
        }

        static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }


    }
}
