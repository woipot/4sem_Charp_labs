using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EightTaskLib;

namespace _08_01_ComplexVector
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var complex1 = new MyComplex(10, 100);
                var complex2 = new MyComplex(0, 10);

                var zeroComplex = new MyComplex(0, 0);
                MyComplex.DeleteEvent += DevisionError1;

                Console.WriteLine($"complex 1 = {complex1}");
                Console.WriteLine($"complex 2 = {complex2}\n");

                var sumComplex = complex1 + complex2;
                Console.WriteLine($"complex 1 + complex 2 = {sumComplex}\n");

                var subComplex = complex1 - complex2;
                Console.WriteLine($"complex 1 - complex 2 = {subComplex}\n");

                var multComplex = complex1 * complex2;
                Console.WriteLine($"complex 1 * complex 2 = {multComplex}\n");

                var divComplex = complex1 / complex2;
                Console.WriteLine($"complex 1 / complex 2 = {divComplex}\n");

                var zerodivision = complex1 / zeroComplex;
                Console.WriteLine($"complex 1 / zeroComplex = {zeroComplex}\n");

                var moduleComplex1 = complex1.GetModule();
                Console.WriteLine($"complex 1 Module = {moduleComplex1}\n");

                var complexRoot = complex1.GetRoot(2);
                foreach (var token in complexRoot)
                {
                    Console.WriteLine($"complex 1  root = {token}");
                }
                Console.WriteLine();

                var complex1Pow = complex1.GetPow(4);
                Console.WriteLine($"complex 1  in 4 degree = {complex1Pow}");

                Console.WriteLine("=========================vector======================\n");

                var vector1 = new MyVector<MyComplex> {complex1, complex2, complex1 * complex2};

                var vector2 = new MyVector<MyComplex> {complex1 - complex2, complex2 + complex1, complex1 * complex2};

                Console.WriteLine($"vector 1 = {vector1}");
                Console.WriteLine($"vector 2 = {vector2}\n");

                var sumVectors = vector1 + vector2;
                Console.WriteLine($"vector1 + vector2 = {sumVectors}\n");

                var subVectors = vector1 - vector2;
                Console.WriteLine($"vector1 - vector2 = {subVectors}\n");

                var vector1Module = vector1.GetModule();
                Console.WriteLine($"vector1 Module = {vector1Module}\n");

                var scalar = vector1.GetScalarMult(vector2);
                Console.WriteLine($"vector1 scalar vector2 = {scalar}\n");


                var vectorList = new List<MyVector<MyComplex>> {vector1, vector2};
                var ortogonaliz = MyVector<MyComplex>.Ortogonalization(vectorList);
                Console.WriteLine("ortoganalization : ");
                var counter = 0;
                foreach (var vector in ortogonaliz)
                {
                    Console.WriteLine($"{++counter}) {vector};");
                }










            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            Console.ReadKey();
        }

        public static void DevisionError1(object sender, MyEvent m)
        {
            Console.WriteLine("\n{0} : {1}", sender, m.Msg);
        }

    }
}
