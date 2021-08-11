using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Terminal.Gui;
using TMAPT.Core;
using TMAPT.Core.Exchanges;
using TMAPT.Core.Intell;
using TMAPT.Core.Intelligence.Position;
using TMAPT.Core.Simulation;

namespace TMAPT.Module
{
    public class ModuleExchange
    {
        private const string moduleName = "Test.dll";
        public static List<string> Console { get; set; } = CoreLib.ConsoleOut;
        public static List<string> Debug { get; set; } = CoreLib.DebugOut;
        public CoreLib Core;
        public ModuleExchange()
        {
        }
        private partial class DynamicImport
        {
            [DllImport(moduleName, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int DynamicLibrary(IntPtr hWnd, String text, String caption, uint type);

            static void LoadDynamicLibrary()
            {
                DynamicLibrary(new IntPtr(0), "", "", 0);
            }
        }
    }
}
