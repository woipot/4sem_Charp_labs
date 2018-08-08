using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace _06_01_dihotomia
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                double result = Root(-1, -10 , EquationFunc, 0.001);
                Console.WriteLine(result);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("#Error" + e.Message);
            }
            Console.ReadKey();
        }

        public delegate double Equation(double x);

        public static double EquationFunc(double x) => x * x*10+x+10;

        public static double Root(double a, double b, Equation eq1, double accuracy)
        {
            var isSameSign = eq1.Invoke(a) * eq1.Invoke(b) >= 0;
            if (isSameSign)
            {
                throw new ArgumentException("#Error: f(a) and f(b) have eq. sign.");
            }

            double result = 0;
            bool shouldContinue;
            do
            {
                result = (a + b) / 2;

                if (eq1.Invoke(a) * eq1.Invoke(b) < 0)
                {
                    b = result;
                }
                else
                {
                    a = result;
                }

                var accuracyNotReached = Math.Abs(eq1.Invoke(result)) > accuracy;
                var limitNotReached = b - a > 0;
                shouldContinue = accuracyNotReached && limitNotReached;
            } while (shouldContinue);

            return result;
        }
    }
}
