using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            _listView.SetSource(CoreLib.ConsoleOut.ToList());

            Win.Add(_listView);

            CoreLib.SoftwareEvents.onUpdate += (sender, e) => Refresh(sender);


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
            _listView.SetSource(CoreLib.ConsoleOut.ToList());
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
