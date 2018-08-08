using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_03_constantsDeterm
{
    public static class ConstantsDetermer
    {

        #region EMethods

        public static decimal Esum(int afterPoint)
        {
            var e = 0m;
            var n = 0;
            var fact = 0m;

            do
            {
                fact = 1 / (decimal)GetFactorial(n);
                e += fact;
                ++n;
            }
            while (IsBiggerThenNeed(fact, afterPoint));

            return Truncate(e, afterPoint);
        }

        public static decimal ESpecialEq(int afterPoint)
        {

            var e = 1m;
            var token = 1m;
            uint n = 1;
            do
            {
                token *= 1m / n;
                e += token;
                n++;
            } while (IsBiggerThenNeed(token, afterPoint));

            return Truncate(e, afterPoint);
        }

        #endregion



        #region PiMethods

        public static decimal PiBailey(int afterPoint)
        {
            decimal pi = 0;

            decimal regToken;
            var counter = 0;

            do
            {
                var coeff = 1 / (decimal) Math.Pow(16, counter);
                var firstToken = 4m / (8 * counter + 1);
                var secondToken = 2m / (8 * counter + 4);
                var thirdToken = 1m / (8 * counter + 5);
                var fourthToken = 1m / (8 * counter + 6);

                regToken = coeff * (firstToken - secondToken - thirdToken - fourthToken);

                pi += regToken; 

                ++counter;
            } while (IsBiggerThenNeed(regToken, afterPoint));

            return Truncate(pi, afterPoint);
        }

        public static decimal PiFromSpecialEq(int afterPoint)
        {
            decimal pi = 0;

            decimal regToken;
            var counter = 0;
            do
            {
                var coeff = (decimal)(Math.Pow(-1, counter) / Math.Pow(2, 10 * counter));
                var firstToken = -32m / (4 * counter + 1);
                var secondToken = 1m / (4 * counter + 3);
                var thirdToken = 256m / (10 * counter + 1);
                var fourthToken = 64m / (10 * counter + 3);
                var fifthToken = 4m / (10 * counter + 5);
                var sixthToken = 4m / (10 * counter + 7);
                var seventhToken = 1m / (10 * counter + 9);

                regToken = coeff * (firstToken - secondToken + thirdToken - fourthToken - fifthToken -
                                         sixthToken + seventhToken);

                pi += regToken;

                ++counter;
            }while (IsBiggerThenNeed(regToken, afterPoint)); // while (counter <= 200);

            var deviser = 64m + (decimal)(1 / Math.Pow(10, afterPoint));
            pi = pi / deviser; 

            return pi;
        }
        #endregion



        #region Ln2Methods
        public static decimal Ln2Sum(int afterPoint)
        {
            decimal ln2 = 0;
            var counter = 1;
            decimal sumToken;

            do
            {
                sumToken = (decimal)(1 / (counter * Math.Pow(2, counter)));
                ln2 += sumToken;
                ++counter;
            } while (IsBiggerThenNeed(sumToken, afterPoint));

            return Truncate(ln2, afterPoint + 1);
        }

        public static decimal Ln2Integral(int afterPoint)
        {
            var n = 1300;
            var h = 1m / n;
            var sum = 0m;
            var x0 = 1m;
            var x1 = 1 + h;
            decimal regToken;

            for (var i = 0; i < n; i++)
            {
                var firstToken = 1m / x0;
                var secondToken = 4m * 1m;
                var thirdToken = x0 + h / 2m;
                var fourthToken = 1m / x1;

                regToken = firstToken + secondToken / thirdToken + fourthToken;
                regToken = h / 6m * regToken;

                x0 += h;
                x1 += h;

                sum += regToken;
            }

            var ln2 = sum;
            return Truncate(ln2,afterPoint);
        }//не подогнанно 
        #endregion



        #region Sqrt2Methods

        public static double Sqrt2FromSpecialEq()
        {
            var sqrt2 = 2 * Math.Cos(Math.PI / 4);
            return sqrt2;
        }

        public static decimal Sqr2FromIterations(int afterPoint)
        {
            decimal sqrt2 = 1;
            decimal check;

            do
            {
                check = Math.Abs(sqrt2 * sqrt2 - 2);
                var newToken = (sqrt2 + 2m / sqrt2) / 2;
                sqrt2 = newToken;
            } while (IsBiggerThenNeed(check, afterPoint));

            return Truncate(sqrt2, afterPoint);
        }

        #endregion



        #region GammaMethods
        public static double GammaFromSpecialEq()
        {
            var gamma = Math.Log(1.7810724179901979852);
            return gamma;
        }

        public static decimal GammaFromIntegral()
        {
            const decimal integralRes = -0.870057726728315506734648m;
            const decimal sqrtPi = 1.772453850905516027298167m;

            var gamma = -2m * (2m * integralRes / sqrtPi + (decimal)Math.Log(2));
            return gamma;

        }
        #endregion



        #region PrivateMethods

        private static bool IsBiggerThenNeed(decimal value, int afterPointCount)
        {
            var tmpVal = 1 / Math.Pow(10, afterPointCount);
            var absoluteValue = (double)Math.Abs(value);

            return absoluteValue > tmpVal;
        }

        private static double GetFactorial(int value)
        {
            var result = 1.0;
            for (var i = 1; i <= value; i++)
            {
                result *= i;
            }
            return result;
        }

        private static decimal Truncate(decimal num, int afterPoint)
        {
            var signnum = Math.Sign(num);
            var absoluteNum = Math.Abs(num);

            var tenDegree = Math.Pow(10, afterPoint);

            var newNum = absoluteNum * (decimal)tenDegree;
            var result = Math.Truncate(newNum);
            result /= (decimal)tenDegree;
            return result;
        }

        #endregion
    }
}
