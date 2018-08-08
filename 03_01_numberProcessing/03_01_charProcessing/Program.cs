using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03_01_charProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputStr;

            if (args.Length == 0 || args[0] == "-c")
            {
                Console.WriteLine("Enter char set: ");
                inputStr = Console.ReadLine();
            }
            else if (args[0] == "-f" && args.Length == 2)
            {
                var sr = File.OpenText(args[1]);
                inputStr = sr.ReadToEnd();
                sr.Dispose();
            }
            else
            {
                throw new Exception("#Error: incorrect arguments");
            }

            var separations = new[] {' '}; 
            var sequence = inputStr?.Split(separations, StringSplitOptions.RemoveEmptyEntries);

            var sum = 0;
            if (sequence.Length != 0)
            {
                foreach (var str in sequence)
                {

                    var localSum = 0;
                    foreach (var c in str)
                    {
                        localSum += (int)c;
                    }

                    sum += localSum;
                }

                Console.WriteLine("Average char = {0}\nAverage = {1}", (char)(sum / sequence.Length), (double)sum/sequence.Length);
            }
            else
            {
                Console.WriteLine("Averag = 0\nEmpty input...");
            }

            Console.ReadKey();
        }
    }
}
