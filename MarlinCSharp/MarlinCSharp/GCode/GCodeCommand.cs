using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharp.GCode
{
    public class GCodeCommand
    {
        public string Command { get; set; } = "";

        public override string ToString()
        {
            return Command;
        }

        public int Checksum(int lineNumber)
        {
            string prefix = "N " + lineNumber.ToString();
            string command = prefix + Command;

            int cs = 0;
            foreach(var c in command.ToCharArray())
            {
                cs = cs ^ c;
            }

            return cs & 0xff;
        }

        public string GetCheckedCommand(int lineNumber)
        {
            return String.Format("N{0} {1}*{2}", lineNumber, Command, Checksum(lineNumber));
        }

    }
}
