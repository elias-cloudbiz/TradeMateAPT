using System;
using System.Collections.Generic;
using System.Text;
using TMPFT.Core;

namespace TMPFT
{
    internal class Import
    {
        public List<string> ConsoleoutList { get; set; } = new List<string>();
        public List<string> ErroroutputList { get; set;  } = new List<string>();
        public List<string> DebugoutputList { get; set; } = new List<string>();

        internal Import() {

            ConsoleoutList = CoreLib.ConsoleoutList;
            ErroroutputList = CoreLib.ErroroutputList;
            DebugoutputList = CoreLib.DebugoutputList;
        
        }


    }
}
