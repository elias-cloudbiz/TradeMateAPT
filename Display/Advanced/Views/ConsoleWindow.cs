using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Terminal.Gui;
using TMPFT.Core;
using TMPFT.Core.Events;

namespace TMPFT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Console", Description: "View Prediction Data and Statistics")]
    [ScenarioCategory("Debug")]
    [ScenarioCategory("Developer")]
    public class ConsoleWindow : Scenarios
    {
        private static ListView ListView;
        public override void Setup()
        {
            ListView = new ListView()
            {
                X = 1,
                Y = 1,
                Height = Dim.Fill(1),
                Width = Dim.Fill(1),
                ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            ListView.SetSource(CoreLib.ConsoleOut.ToList());

            Win.Add(ListView);

            EventsReporter.SoftwareEvents.onScreenUpdate += (sender, e) => Refresh(sender);
            EventsReporter.SoftwareEvents.onStartUpComplete += (sender, e) => RequestStop();

            CreateStatusBar();

        }

        private void CreateStatusBar()
        {
            /*            var statusBar = new StatusBar(new StatusItem[] {
                                       new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh", null),
                                       new StatusItem(Key.CtrlMask | Key.S, "~^S~ Sync",  () => Refresh("Eee")),
                                       new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => StopApplication()),
                                   });
                        statusBar.ColorScheme = Colors.TopLevel;
                        Top.Add(statusBar);*/
        }

        public void Refresh(object sender)
        {
            ListView.SetSource(CoreLib.ConsoleOut.ToList());
        }
        public override void RequestStop()
        {
            // Request 
            Thread.Sleep(1000);
            base.RequestStop();
            RunStateMachine.CreateScenario(1);

        }
        public void ClearWindow()
        {
            CoreLib.ConsoleOut.Clear();
        }

        public void StopApplication()
        {
            Application.RequestStop();
        }





    }
}
