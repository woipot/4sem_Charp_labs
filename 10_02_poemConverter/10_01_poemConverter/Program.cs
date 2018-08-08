using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10_01_poemConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
                throw new Exception("too few arguments");
            //StreamWriter sr = File.CreateText("test1");
            Poem test1 = new Poem(@args[0], @args[1], @args[2]);
            Console.WriteLine(test1.ProcPoem);
            Console.ReadKey();
            //string str = Poem.WordFromDictionary( 2,@"D:\Projects\c#\10_01_poemConverter\10_01_poemConverter\newDictionary.txt");
        }
    }
}
