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

        public byte Checksum(int lineNumber)
        {
            string prefix = "N " + lineNumber.ToString();
            string command = prefix + Command;

            byte mask = 0xff;
            byte cs = 0;
            foreach(var c in Encoding.ASCII.GetBytes(command))
            {
                cs = (byte)(cs ^ c);
            }

            return (byte)(cs & mask);
        }

        public string GetCheckedCommand(int lineNumber)
        {
            return String.Format("N{0} {1}*{2}", lineNumber, Command, Checksum(lineNumber));
        }

    }
}
