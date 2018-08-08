using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03_01_baseTranslate
{
    class Program
    { 
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("too few Argumets");
                Console.ReadKey();
                Environment.Exit(0);
               // throw new Exception("too few Argumets");
            }

            var isNumber = int.TryParse(args[0], out var number);
            var isSystem = int.TryParse(args[1], out var systemBase);

            if (isNumber && isSystem)
            {
                if (systemBase < 2 || systemBase > 36)
                {
                    Console.WriteLine("");
                    throw new Exception("#Error: invalid system"); 
                }
                
                //string newNumber = Convert.ToString(number, systemBase);
                var numberSign = Math.Sign(number);
                number = Math.Abs(number);

                var newNumber = ReverseGorner(number, systemBase);

                Console.WriteLine(
                    $"You enter: {number}[10] to {{x}}[{systemBase}]  =  {numberSign}*{newNumber}[{systemBase}]");
            }
            else
            {
                Console.WriteLine("#Error: invalid Arguments");
                throw new Exception("#Error: invalid Arguments");
            }

            Console.ReadKey();
        }

        public static string ReverseGorner(int number, int systemBase)
        {
            var output = new StringBuilder();
            var copiedInitialNumber = number;
            while (copiedInitialNumber != 0)
            {
                output.Append((copiedInitialNumber % systemBase < 10)
                    ? (char) (copiedInitialNumber % systemBase + '0')
                    : (char) (copiedInitialNumber % systemBase + 'A' - 10));
                copiedInitialNumber /= systemBase;
            }

            return ReverseArrayFramework(output.ToString());
        }

        private static string ReverseArrayFramework(string str)
        {
            var arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
