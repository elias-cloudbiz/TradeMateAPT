using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;
using TMPFT.Core;

namespace TMPFT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Startup", Description: "View Prediction Data and Statistics")]
    [ScenarioCategory("Debug")]
    [ScenarioCategory("Developer")]
    public class ConsoleWindow : Scenarios
    {
        private static ListView _listView;
        public static bool StartupCompleted = false;
        public override void Setup()
        {
            _listView = new ListView()
            {
                X = 1,
                Y = 1,
                Height = Dim.Fill(1),
                Width = Dim.Fill(1),
                ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            _listView.SetSource(CoreLib.ConsoleOutputList);

            Win.Add(_listView);

            CoreLib.Exchange.onPublicComplete += (sender, e) => ConsoleAddLine(e.Value);

            var statusBar = new StatusBar(new StatusItem[] {
                           new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh", null),
                           new StatusItem(Key.CtrlMask | Key.S, "~^S~ Sync",  () => ConsoleAddLine("Eee")),
                           new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => StopApplication()),
                       });
            statusBar.ColorScheme = Colors.TopLevel;
            Top.Add(statusBar);
        }

        public void ConsoleAddLine(string e)
        {
          Application.Refresh();
        }

        public void ClearWindow()
        {
            CoreLib.ConsoleOutputList.Clear();
        }

        public void StopApplication()
        {
            Application.RequestStop();
        }





    }
}
