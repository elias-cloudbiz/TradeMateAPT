using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Terminal.Gui;
using TMPFT.Core;

namespace TMPFT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Debug", Description: "View Prediction Data and Statistics")]
    [ScenarioCategory("Debug")]
    [ScenarioCategory("Developer")]
    public class DebugWindow : Scenarios
    {
        private static ListView _listView;

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

            _listView.SetSource(CoreLib.DebugOut.ToList());

            Win.Add(_listView);

            CoreLib.SoftwareEvents.onScreenUpdate += (sender, e) => Refresh(sender);
        }

        public void Refresh(object sender)
        {
            _listView.SetSource(CoreLib.DebugOut.ToList());
        }
        private void CreateStatusBar()
        {
            /*
             var statusBar = new StatusBar(new StatusItem[] {
                        new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh", null),
                        new StatusItem(Key.CtrlMask | Key.S, "~^S~ Sync",  () => Refresh("Eee")),
                        new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => StopApplication()),
                    });
            statusBar.ColorScheme = Colors.TopLevel;
            Top.Add(statusBar);
            */
        }
        public static void clearLines()
        {
            CoreLib.DebugOut.Clear();
        }
        public static void stopApplication()
        {
            Application.RequestStop();
        }

    }
}
