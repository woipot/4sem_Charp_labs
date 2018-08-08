using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_01_fractionsTranslation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("too few Argumets");
            }

            string[] strArrSplitedInputNumbers;

            var spaceSeparator = new[] { ' ' };
            switch (args[0])
            {
                case "-c":
                    Console.WriteLine("Enter fraction and system base [n/m system base]:  ");
                    var userInput = Console.ReadLine();
                    strArrSplitedInputNumbers = userInput?.Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                    break;

                case "-f" when args.Length == 2:
                    var filePath = args[1];
                    var fileData = File.ReadAllText(filePath);
                    strArrSplitedInputNumbers = fileData.Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                    break;

                default:
                    throw new Exception("#Error: Incorrect arguments");
            }

            if (strArrSplitedInputNumbers != null
                && strArrSplitedInputNumbers.Length < 2)
                throw new Exception("#Error: There are no arguments");

            var fraction = strArrSplitedInputNumbers?[0];
            var strArrFraction = fraction?.Split('/');

            int numerator =0;
            int denominator =0;
            var isIntNumber = int.TryParse(strArrFraction?[0], out numerator) &&
                               int.TryParse(strArrFraction?[1], out denominator);
            if (!isIntNumber)
            {
                throw new Exception("#Error: Incorrect fraction");
            }

            var isCorrectSystemBase = int.TryParse(strArrSplitedInputNumbers?[1], out var systemBase);

            if (systemBase < 2 || systemBase > 36 || !isCorrectSystemBase)
            {
                throw new Exception("#Error: System base out of range (2 <= system base <= 36) or incorect value");
            }

            var sign = true;
            if (numerator < 0)
            {
                sign = false;
                numerator = Math.Abs(numerator);
            }

            var numeratorDevidOnDenominator =  (double)numerator / denominator;

            var integerPart = Math.Abs(Math.Truncate(numeratorDevidOnDenominator));
            var fractionPart = numeratorDevidOnDenominator - integerPart;

            var newNumerator = (int)(numerator - integerPart * denominator);

            // 1 строчка 1 действие
            var beforePoint = (int)integerPart != 0 ? ReverseGorner((int)integerPart, systemBase) + ".": "0.";



            var afterPoint = new StringBuilder();
            var listAfterPointInteger = new List<int>();

            if (!fractionPart.Equals(0))
            {
                var counterOfDiscardedNumbers = 0;
                var numeratorCopy = newNumerator;

                while (counterOfDiscardedNumbers < denominator)
                {
                    var addition = systemBase * numeratorCopy;
                    listAfterPointInteger.Add(addition / denominator);
                    numeratorCopy = (addition) % denominator;
                    ++counterOfDiscardedNumbers;
                }

                var lastTermOfSequence = numeratorCopy;
                var listNumberOfPriod = new List<int>();
                var periodLength = 0;

                do
                {
                    listNumberOfPriod.Add((systemBase * numeratorCopy) / denominator);
                    numeratorCopy = (systemBase * numeratorCopy) % denominator;
                    ++periodLength;
#if DEBUG
                    Console.WriteLine("numeratorCopy = {0}\tperiodLength = {1}", numeratorCopy, periodLength);
#endif
                } while (numeratorCopy != lastTermOfSequence);


                var beforePeriodLength = 0;
                var tmpCounter = 0;
                foreach (var number in listAfterPointInteger)
                {
                    if (number == listNumberOfPriod[tmpCounter])
                    {
                        ++tmpCounter;
                        if (tmpCounter == listNumberOfPriod.Count)
                            break;
                    }
                    else
                    {
                        if (tmpCounter != 0)
                        {
                            beforePeriodLength += tmpCounter;
                            tmpCounter = 0;
                        }

                        ++beforePeriodLength;
                    }
                }

                var fullLength = beforePeriodLength + periodLength;
                for (var i = 0; i < fullLength; i++)
                {
                    if (i == beforePeriodLength)
                        afterPoint.Append(" (");
                  
                    var number = listAfterPointInteger[i];
                    var charNumber = NumberInSystemChar(number);

                    afterPoint.Append(charNumber);
                }
                afterPoint.Append(")");

            }

            if (!sign)
            {
                beforePoint = beforePoint.Insert(0, "-");
            }

            Console.WriteLine("You enter {0} -> {1} [{2}]", fraction, beforePoint + afterPoint, systemBase);
            Console.ReadKey();
        }

        public static string ReverseGorner(int number, int systemBase)
        {
            var output = new StringBuilder();
            var copiedInitialNumber = number;
            while (copiedInitialNumber != 0)
            {
                output.Append((copiedInitialNumber % systemBase < 10) ?
                    (char)(copiedInitialNumber % systemBase + '0') :
                    (char)(copiedInitialNumber % systemBase + 'A' - 10));
                copiedInitialNumber /= systemBase; //Обратная схема Горнера 
            }

            return ReverseArrayFramework(output.ToString());
        }

        private static string ReverseArrayFramework(string str)
        {
            var arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        private static char NumberInSystemChar(int number)
        {
            if (number < 10)
            {
                return number.ToString()[0];
            }
            else
            {
                return (char)('A' + number - 10);
            }
        }
    }
}
