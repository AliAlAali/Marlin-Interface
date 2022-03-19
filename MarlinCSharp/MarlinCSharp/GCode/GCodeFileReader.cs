using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MarlinCSharp.GCode
{
    public class GCodeFileReader : IEnumerable<GCodeCommand>
    {
        public static string[] SUPPORT_EXTENSIONS = new string[1] { ".gcode"};
        public List<GCodeCommand> Commands { get; } = new List<GCodeCommand>();


        /// <summary>
        /// Primitive way to read a gcode file. THIS METHOD DOES NOT CHECK THE INTEGRITY OF GCODE NOR OMMITS COMMENTS.
        /// </summary>
        /// <param name="path">Path to the gcode file</param>
        /// <returns>GCode file reader</returns>
        public static GCodeFileReader Open(string path)
        {
            var extension = Path.GetExtension(path);
            if(SUPPORT_EXTENSIONS.ToList().Contains(extension))
            {
                throw new FileLoadException($"Unsupported file type: {extension}");
            }

            var lines = File.ReadAllLines(path);
            var gfile = new GCodeFileReader();
            lines.ToList().ForEach(l => gfile.Commands.Add(new GCodeCommand() { Command = l }));

            return gfile;
        }

        public IEnumerator<GCodeCommand> GetEnumerator()
        {
            return Commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
