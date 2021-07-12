using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;
using TMPFT.Core;

namespace TMPFT
{
    public class ModuleImport
    {

        public List<string> ConsoleoutList { get; set; } = new List<string>();
        public List<string> ErroroutputList { get; set; } = new List<string>();
        public List<string> DebugoutputList { get; set; } = new List<string>();

        public ModuleImport()
        {
            ConsoleoutList = CoreLib.ConsoleOutputList;
            ErroroutputList = CoreLib.ErroroutputList;
            DebugoutputList = CoreLib.DebugOutputList;





        }
    }
}
