using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;
using TMAPT.Core;

namespace TMAPT.Module
{
    public class ModuleExchange
    { 
        public static List<string> Console { get; set; } = new List<string>();
        public static List<string> Debug { get; set; } = new List<string>();

        public ModuleExchange()
        {
            Console = CoreLib.ConsoleOut.ToList();
            Debug = CoreLib.DebugOut.ToList();
        }

        internal object getConsoleList()
        {
            return CoreLib.getConsoleList();
        }

        public partial class Events
        {
            public partial class Software { }

        }
    }
}
