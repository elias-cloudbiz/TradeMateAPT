using System;
using System.Collections.Generic;
using System.Text;
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

            _listView.SetSource(CoreLib.DebugOutputList);


            Win.Add(_listView);

/*            var statusBar = new StatusBar(new StatusItem[] {
                                   new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh", ()  => coreLib.Start()),
                                   new StatusItem(Key.CtrlMask | Key.S, "~^S~ Sync",  () => DebugAddline("Eee")),
                                   new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", null),
                               });
            statusBar.ColorScheme = Colors.TopLevel;
            Top.Add(statusBar);*/



            CoreLib.onUpdate += (sender, e) => DebugAddline(e.Value);

        }

        public void DebugAddline(string e)
        {
            Application.Refresh();
        }

        public static void clearLines()
        {
            CoreLib.DebugOutputList.Clear();

        }

        public static void stopApplication()
        {
            Application.RequestStop();
        }

    }
}
