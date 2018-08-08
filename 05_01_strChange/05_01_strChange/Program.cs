using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05_01_strChange
{
    class Program
    {
        static void Main(string[] args)
        {

            string inputStr;

            if (args.Length == 0  || args[0] == "-c")
            {
                Console.Write("Enter str: ");
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
               Console.WriteLine("Error Arguments");
                Console.ReadKey();
                return;
            }
            

            var separators = new[] {' '};

            var sequence = inputStr.Split(separators, StringSplitOptions.RemoveEmptyEntries); 

            //sort and out word, which consist of last letters 
            var lastLettersWord = StringEditor.SortOutLastLetters(sequence); 
            Console.WriteLine("\nWord, which consist of last letters = {0}", lastLettersWord);

            //up register first letter and down register last lettet
            var UpFirstDownLastStr = StringEditor.UpFirstDownLast(inputStr);
            Console.WriteLine("up register first letter and down register last lettet: {0}", UpFirstDownLastStr);

            // find Word 
            Console.Write("\nEnter searching word: ");
            var searchingWord = Console.ReadLine();
            var meetCount = StringEditor.MeetCount(inputStr, searchingWord);
            Console.WriteLine("This word occurs {0} times", meetCount);

            //Replace penultimate word
            Console.Write("\nEnter new penultimate word: ");
            var penultimateWord = Console.ReadLine();
            try
            {
                var newStr = StringEditor.ReplacePenultimateWord(inputStr, penultimateWord);
                Console.WriteLine("Relace penultimate to Word: {0}", newStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //find k word, which has upper first letter
            Console.Write("\nEnter number: ");
            if (int.TryParse(Console.ReadLine(), out var enterNumber))
            {
                enterNumber = Math.Abs(enterNumber);
                var kWord = StringEditor.ReturnUpWord(inputStr, enterNumber);
                Console.WriteLine("U word = {0}[{1}]", kWord, enterNumber);
            }
            else 
                Console.WriteLine("#Error: incorrect number");


            Console.ReadKey();
        }
    }
}
