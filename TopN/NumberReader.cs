using System;
using System.IO;

namespace TopN
{
    /// <summary>
    /// Extends the StreamReader with a method to read a number from a line
    /// </summary>
    class NumberReader : StreamReader
    {
        public NumberReader(Stream stream)
            : base(stream)
        {
        }

        public NumberReader(string filename)
            : base(filename)
        {
        }

        // Consider outputting if EOF or invalid
        public int ReadNumber()
        {
            return Int32.Parse(ReadLine());
        }
    }
}
