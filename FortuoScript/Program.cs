using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FortuoScript
{
    class Program
    {
        const string Version = "";

        static void Main(string[] args)
        {
            FortuoInterpreter fi = new FortuoInterpreter();

            if (args.Length >= 2)
            {
                if (args[0].Equals("-f"))
                {
                    string path = args[1];
                    fi.ExecuteFile(path);
                    
                }
            }

            while (true)
            {
                Console.Write("FS> ");
                string input = Console.ReadLine();
                if (input.Equals("quit"))
                    return;

                try
                {
                    fi.Execute(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: \r\n{0}", ex.Message);
                }
                Console.WriteLine();
            }
        }
    }
}