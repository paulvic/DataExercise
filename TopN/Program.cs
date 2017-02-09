using System;

namespace TopN
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 1;

            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            if (String.IsNullOrEmpty(args[0]))
            {
                PrintUsage();
                return;
            }

            if (!Int32.TryParse(args[1], out n))
            {
                PrintUsage();
                return;
            }

            var sorter = new FileSorter();                        
            sorter.TopN(args[0], n);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Outputs the TOP N largest numbers in a given file");
            Console.WriteLine("USAGE:");
            Console.WriteLine("  TOPN.exe <path to file containing numbers> <number of records to be output>");
        }
    }
}
