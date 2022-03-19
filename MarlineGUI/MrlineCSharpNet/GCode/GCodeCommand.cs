using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharpNet.GCode
{
    public class GCodeCommand
    {
        public string Command { get; set; } = "";

        public override string ToString()
        {
            return Command;
        }
    }
}
