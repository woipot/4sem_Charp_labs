using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_03_constantsDeterm
{
    class Program
    {
        static void Main(string[] args)
        {

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var sumE = ConstantsDetermer.Esum(20);
                Console.WriteLine($"1) E from sum        = {sumE:F20}");

                var specialEqE = ConstantsDetermer.ESpecialEq(15);
                Console.WriteLine($"2) E from Sprcial eq = {specialEqE:F15}\n");




                var piBailey = ConstantsDetermer.PiBailey(15);
                Console.WriteLine($"1) Pi from Bailey eq  = {piBailey:F15}");

                var specialEqPi = ConstantsDetermer.PiFromSpecialEq(15);
                Console.WriteLine($"2) Pi from Special eq = {specialEqPi:F15}\n");




                var sumLn2 = ConstantsDetermer.Ln2Sum(15);
                Console.WriteLine($"1) ln2 from sum      = {sumLn2:F15}");

                var integralLn2 = ConstantsDetermer.Ln2Integral(15);
                Console.WriteLine($"2) ln2 from integral = {integralLn2:F15}\n");



                var specialEqSqrt2 = ConstantsDetermer.Sqrt2FromSpecialEq();
                Console.WriteLine($"1) Sqrt(2) from Special eq  = {specialEqSqrt2:E15}");

                var iterationsSqrt2 = ConstantsDetermer.Sqr2FromIterations(15);
                Console.WriteLine($"2) Sqrt(2) from Iteraations = {iterationsSqrt2:F15}\n");



                var specialEqGamma = ConstantsDetermer.GammaFromSpecialEq();
                Console.WriteLine($"1) Gamma from Special eq  = {specialEqGamma:E14}");

                var integralGamma = ConstantsDetermer.GammaFromIntegral();
                Console.WriteLine($"2) Gamma from integral = {integralGamma:F15}\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine($"Working Time: {ts.TotalSeconds}");

            Console.ReadKey();
        }
    }
}
