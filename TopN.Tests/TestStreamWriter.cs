using System.Collections.Generic;
using System.IO;

namespace TopN.Tests
{
    /// <summary>
    /// A StreamWriter class to that redirects Writes into an array so that the data can be inspected
    /// </summary>
    class TestStreamWriter : StreamWriter
    {
        List<int> data;
        public int[] Data
        {
            get { return data.ToArray(); }
            private set { }
        }

        public TestStreamWriter(Stream stream)
        : base(stream)
        {
            data = new List<int>();
        }

        public override void WriteLine(int value)
        {
            data.Add(value);
        }
    }
}
