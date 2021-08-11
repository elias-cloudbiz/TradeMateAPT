using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Terminal.Gui;
using TMAPT.Core.Events;
using TMAPT.Module;

namespace TMAPT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Console", Description: "View Prediction Data and Statistics")]
    [ScenarioCategory("Debug")]
    [ScenarioCategory("Developer")]
    public class ConsoleWindow : Scenario
    {
        public ConsoleWindow() : base() { }

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

            ListView.SetSource(ModuleExchange.Console);

            Win.Add(ListView);

            Event.Software.onScreenUpdate += (sender, e) => Refresh(sender);
            Event.Software.onStartUpComplete += (sender, e) => RequestStop();

            CreateStatusBar();

            base.Setup();

            //r.Width = ListView.Bounds.Width;
            //r.Height = ListView.Bounds.Height;
        }

        private void CreateStatusBar()
        {
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.CtrlMask | Key.R, "~^R~ Construct Module", () => throw new Exception()),
                new StatusItem(Key.CtrlMask | Key.S, "~^S~ Predictor",  () => throw new Exception())
            });

            statusBar.ColorScheme = Colors.TopLevel;
            Top.Add(statusBar);
        }

        public void Refresh(object sender)
        {
            Application.Refresh();
        }
        public override void RequestStop()
        {
            // Request 
            //Thread.Sleep(1000);
            base.RequestStop();
            //RunStateMachine.CreateScenario(1);

        }
        public void ClearWindow()
        {
            ModuleExchange.Console.Clear();
        }

        public void StopApplication()
        {
            Application.RequestStop();
        }
    }
}
