using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;

namespace TMPFT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Console", Description: "View Prediction Data and Statistics")]
    [ScenarioCategory("Debug/Developer")]
    public class ConsoleWindow : Scenarios
    {
        private static Import Module = new Import();
        private static ListView _listView;
        private static List<string> consoleOutput = new List<string>();
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
            consoleOutput = new List<string>() { "Console output initialized ... " };

            _listView.SetSource(Module.ConsoleoutList);

            Win.Add(_listView);

/*           
 *           var statusBar = new StatusBar(new StatusItem[] {
                       new StatusItem(Key.CtrlMask | Key.R, "~^R~ Refresh", ()  => writeLine("new")),
                       new StatusItem(Key.CtrlMask | Key.S, "~^R~ Sync", null),
                       new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", null),
                   });
            statusBar.ColorScheme = Colors.TopLevel;
            Top.Add(statusBar);
*/
        }

        public static void writeLine(string line)
        {
            //consoleOutput.Add(line);

            _listView.SetSource(consoleOutput);
            //Top.SetNeedsDisplay();
        }

        public static void clearLines() {
            consoleOutput.Clear();
           
        }

        public static void stopApplication() {
            Application.RequestStop();
        }

    }
}
