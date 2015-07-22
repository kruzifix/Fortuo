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
                    string file = args[1];
                    if (File.Exists(file))
                    {
                        string[] lines = File.ReadAllLines(file);
                        foreach (var l in lines)
                            fi.Execute(l);
                    }
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