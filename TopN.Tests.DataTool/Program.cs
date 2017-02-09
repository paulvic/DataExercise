using System;
using System.IO;

namespace TopN.Tests.DataTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = Path.GetTempFileName();

            int lines = 1000000;
            if (args.Length > 0)
            {
                if ((!Int32.TryParse(args[0], out lines)) || (lines < 1))
                {
                    lines = 1000000; // default is a million
                }
            }
            else
            {
                PrintUsage();
                return;
            }

            using (StreamWriter fileWriter = new StreamWriter(fileName))
            {
                Random rnd = new Random();
                for (int i = 0; i < lines; i++)
                {
                    int rndnumber = rnd.Next();
                    fileWriter.WriteLine(rndnumber);
                }
            }

            Console.WriteLine($"File with {lines} line(s) created: {fileName}");
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Generates a file containing N random numbers and outputs the file path");
            Console.WriteLine("USAGE:");
            Console.WriteLine("  TopN.Tests.DataTool.exe <number of elements to be generated>");
        }
    }
}
