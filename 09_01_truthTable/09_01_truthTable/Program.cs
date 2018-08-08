using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _09_01_truthTable
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter file path: ");
            var filePath = Console.ReadLine();

            try
            {
                var sr = File.OpenText(filePath ?? throw new Exception("#Error: File not Exist"));
           
           
                var stringFromFile = sr.ReadLine();

                var examplesCounter = 1;
                while (stringFromFile != null)
                {
                    Console.WriteLine($"{examplesCounter}. {stringFromFile}\n");
                    var answer = Calculate(stringFromFile, out var variablesDictioary);
                    Console.Write("\n\n");

                    ++examplesCounter;
                    stringFromFile = sr.ReadLine();
                }


                sr.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
            Console.ReadLine();
        }

        public static IEnumerable<bool> Calculate(string inputStr, out IDictionary<string, List<bool>>variablesDictionary)
        {
            var separators = new[] {' '};
            var splitedInputStr = inputStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);


            var postfix = InPostfix(splitedInputStr);

            variablesDictionary = FillingVariables(splitedInputStr);

            try
            {
                var answer = CalculatePostfix(postfix, variablesDictionary);
                var sknf = GetSKNF(variablesDictionary, answer);
                var sdnf = GetSDNF(variablesDictionary, answer);

                Console.WriteLine($"sdnf = {sdnf}\n");
                Console.WriteLine($"sknf = {sknf}");


                return answer;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }




        //private======================================================================================================
        private static IEnumerable<string> InPostfix(IEnumerable<string> splitedInputStr)
        {

            var outputList = new List<string>();

            var operationStack = new Stack<string>();

            foreach (var token in splitedInputStr)
            {
                var isOperation = GetPriority(token) != 0;

                if (isOperation)
                {
                    var isEmptyStack = operationStack.Count == 0;
                    var isOpenBracket = false;
                    var isBiggerPriority = false;

                    if (!isEmptyStack)
                    {
                        var oper = operationStack.Peek();
                       
                        isOpenBracket = oper == "(";

                        var operPriority = GetPriority(oper);
                        var tokenPriority = GetPriority(token);
                        isBiggerPriority = tokenPriority > operPriority;

                    }

                    if (isEmptyStack || isOpenBracket || isBiggerPriority)
                    {
                        operationStack.Push(token);
                    }
                    else
                    {
                        var oper = operationStack.Peek();
                        while (operationStack.Count != 0 && GetPriority(token) <= GetPriority(oper) && oper != "(")
                        {
                            oper = operationStack.Pop();
                            outputList.Add(oper);
                        }
                        
                    }

                }
                else if (token == "(")
                {
                    operationStack.Push(token);
                }
                else if (token == ")")
                {
                    while ( operationStack.Peek() != "(")
                    {
                        var oper = operationStack.Pop();
                        outputList.Add(oper);
                    }

                    operationStack.Pop();
                }
                else
                {
                    outputList.Add(token);
                }
            }

            while (operationStack.Count != 0)
            {
                var oper = operationStack.Pop();
                outputList.Add(oper);
            }


            return outputList;
        }
    
        private static IEnumerable<bool> CalculatePostfix(IEnumerable<string> splitPostfix, IDictionary<string, List<bool>> variablesDictionary)
        {
            var operandStack = new Stack<List<bool>>();

            var operKey = new Stack<string>();

            var intermediateAnswersList = new List<KeyValuePair<string, List<bool>>>();

            foreach (var token in splitPostfix)
            {
                var isOperator = GetPriority(token) != 0;
                
                if (isOperator)
                {
                    List<bool> firstOperand;
                    List<bool> secondOperand;
                    var key = token; 

                    try
                    {
                        if (token != "!")
                        {
                            secondOperand = operandStack.Pop();
                            key += operKey.Pop();
                        }
                        else
                        {
                            secondOperand =null;
                            key = token + operKey.Peek();
                        }

                        firstOperand = operandStack.Pop();
                        

                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    for (var i = 0; i < firstOperand.Count; ++i)
                    {
                        switch (token)
                        {
                            case "&&":
                                firstOperand[i] &= secondOperand[i];
                                break;

                            case "||":
                                firstOperand[i] |= secondOperand[i];
                                break;

                            case "<>":
                                firstOperand[i] = firstOperand[i] == secondOperand[i];
                                break;

                            case "+":
                                firstOperand[i] ^= secondOperand[i];
                                break;

                            case "->":
                                firstOperand[i] = !firstOperand[i] | secondOperand[i];
                                break;

                            case @"/\":
                                firstOperand[i] = !(firstOperand[i] | secondOperand[i]);
                                break;

                            case @"\/":
                                firstOperand[i] = !(firstOperand[i] & secondOperand[i]);
                                break;

                            case "!":
                                firstOperand[i] = !firstOperand[i];
                                break;

                        }
                    }


                    var intermediateList = new List<bool>(firstOperand);
                    var pair = new KeyValuePair<string, List<bool>>(key, intermediateList); 
                    intermediateAnswersList.Add(pair);
                    operandStack.Push(firstOperand);
                    
                }
                else
                {
                    try
                    {
                        var variable = new List<bool>(variablesDictionary[token]);
                        operKey.Push(token);
                        operandStack.Push(variable);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    
                }
            }

            PrintTable(variablesDictionary, intermediateAnswersList);
            return operandStack.Peek();
        }

        private static IDictionary<string, List<bool>> FillingVariables(IEnumerable<string> inputString)
        {

            var time = new Stopwatch();
            time.Start();
            var variablesNameEnumerable = DifferentVariables(inputString);
            time.Stop();
            Console.WriteLine($"\n\nWorking Time: {time.Elapsed.TotalSeconds}");

            var variablesNameList = variablesNameEnumerable.ToList();

            var numberOfRowsInTable = Math.Pow(2, variablesNameList.Count());
            var initDicrtionary = new Dictionary<string, List<bool>>();

            var counter = 1;
            foreach (var str in variablesNameList)
            {
                initDicrtionary[str] = new List<bool>();

                var blockCount = Math.Pow(2, counter);
                var membersOfblockCount = numberOfRowsInTable / blockCount;

                var param = false;

                for (var currentBlock = 0; currentBlock < blockCount; ++currentBlock)
                {
                    for (var i = 0; i < membersOfblockCount; ++i)
                    {
                        initDicrtionary[str].Add(param);
                    }

                    param = !param;
                }

                ++counter;
            }

            return initDicrtionary;
        }

        private static IEnumerable<string> DifferentVariables(IEnumerable<string> inputString)
        {
            var variablesNameList = new List<string>();

            foreach (var c in inputString)
            {
                var isLatterAndFirstTime = char.IsLetter(c[0]) && !variablesNameList.Contains(c);
                if (isLatterAndFirstTime)
                {
                    variablesNameList.Add(c);
                }
            }

            return variablesNameList;
        }

        private static string GetSKNF(IDictionary<string, List<bool>> filDictionary,
            IEnumerable<bool> result)
        {
            var outputStr = new StringBuilder();

            var rowsCounter = 0;
            foreach (var resultValue in result)
            {
                if (resultValue == false)
                {
                    if (rowsCounter != 0)
                        outputStr.Append(" ^ ");

                    outputStr.Append("(");
                    var tmpCounter = 0;
                    foreach (var pair in filDictionary)
                    {
                        var variable = pair.Value[rowsCounter];

                        if (tmpCounter != 0)
                            outputStr.Append(" V ");
                        if (variable == false)
                        {
                            outputStr.Append(pair.Key);
                        }
                        else
                        {
                            outputStr.Append("!");
                            outputStr.Append(pair.Key);
                        }

                        ++tmpCounter;
                    }

                    outputStr.Append(")");
                }
                    ++rowsCounter;
            }

            return outputStr.ToString();
        }

        private static string GetSDNF(IDictionary<string, List<bool>> filDictionary,
            IEnumerable<bool> result)
        {
            var outputStr = new StringBuilder();

            var rowsCounter = 0;
            foreach (var resultValue in result)
            {
                if (resultValue == true)
                {
                    if (rowsCounter != 0)
                        outputStr.Append(" V ");

                    outputStr.Append("(");
                    var tmpCounter = 0;
                    foreach (var pair in filDictionary)
                    {
                        var variable = pair.Value[rowsCounter];

                        if (tmpCounter != 0)
                            outputStr.Append(" ^ ");
                        if (variable == true)
                        {
                            outputStr.Append(pair.Key);
                        }
                        else
                        {
                            outputStr.Append("!");
                            outputStr.Append(pair.Key);
                        }

                        ++tmpCounter;
                    }
                    outputStr.Append(")");
                }
                   ++rowsCounter;
            }

            return outputStr.ToString();
        }

        private static int GetPriority(string operation)
        {
            switch (operation)
            {
                case "<>":
                case "+":
                    return 1;

                case "->":
                    return 2;

                case "||":
                    return 3;

                case "&&":
                case @"/\":
                case @"\/":
                    return 4;

                case "!":
                    return 5;

                default:
                    return 0;
            }
        }

        private static void PrintTable(IDictionary<string, List<bool>> filDictionary
            , IList<KeyValuePair<string, List<bool>>> intermediateAnswer)
        {
            var tableHight = intermediateAnswer[intermediateAnswer.Count - 1].Value.Count;
            var tableLength = filDictionary.Count + intermediateAnswer.Count;


            const char verticalLine = '║';
            const char horizontalLine = '═';

            var sbToPrint = new StringBuilder();

            var elemCounter = 1;

            sbToPrint.Append(verticalLine);
            foreach (var pair in filDictionary)
            {
                sbToPrint.Append($"{pair.Key}{verticalLine}");
                elemCounter += pair.Key.Length + 1;
            }

            foreach (var pair in intermediateAnswer)
            {
                sbToPrint.Append($" {pair.Key}{verticalLine}");
                elemCounter += pair.Key.Length + 2;
            }
            sbToPrint.Append("\n");

            for (var i = 0; i < elemCounter; ++i)
            {
                sbToPrint.Append($"{horizontalLine}");
            }
            sbToPrint.Append("\n");


            for (var hight = 0; hight < tableHight; ++hight)
            {
                sbToPrint.Append(verticalLine);
                int variable;
                foreach (var pair in filDictionary)
                {
                    variable = pair.Value[hight] ? 1 : 0;
                    sbToPrint.Append($"{variable}{verticalLine}");
                }

                foreach (var pair in intermediateAnswer)
                {
                    variable = pair.Value[hight] ? 1 : 0;

                    var gaps = (pair.Key.Contains("!")) ? " " : "  ";
                    sbToPrint.Append($"{gaps}{variable} {verticalLine}");
                }
                sbToPrint.Append("\n");
                Console.Write(sbToPrint);
                sbToPrint.Clear();
            }

            for (var i = 0; i < elemCounter; ++i)
            {
                sbToPrint.Append($"{horizontalLine}");
            }
            sbToPrint.Append("\n");
            Console.Write(sbToPrint);
        }
    }
}

