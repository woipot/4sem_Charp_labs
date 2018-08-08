using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;



namespace _01_01_cub
{
    class Program
    {
        static void Main(string[] args)
        {
            Complex[] karanoTest = CubicEquation.Kordan(1, -4, 5, -2);
            Complex[] vietaTest  = CubicEquation.Vieta(1, -4, 5, -2);
            Console.ReadKey();
        }
    }
}
