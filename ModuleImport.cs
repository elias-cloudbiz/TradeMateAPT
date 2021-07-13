using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;
using TMPFT.Core;

namespace TMPFT
{
    public class ModuleImport
    {

        public static List<string> ConsoleoutList { get; set; } = new List<string>();
        public static List<string> DebugoutputList { get; set; } = new List<string>();

        public ModuleImport()
        {
            ConsoleoutList = CoreLib.ConsoleOut.ToList();
            DebugoutputList = CoreLib.DebugOut.ToList();





        }
    }
}
