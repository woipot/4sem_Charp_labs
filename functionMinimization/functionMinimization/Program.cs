using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace functionMinimization
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter function result");
            var userInput = Console.ReadLine();

            var functionResultList = FunctionResultParce(userInput);

            var sdnf = GetSDNF(functionResultList);

            var simplesdnf = SimplifySDNF(sdnf);

            var sb = new StringBuilder();
            foreach (var node in simplesdnf)
            {
                sb.Append("(");
                foreach (var elem in node)
                {
                    if (elem.Value == false)
                        sb.Append("!");
                    sb.Append((elem.Key + 1) + " ");
                }

                sb.Append(")");
                if (node != simplesdnf.Last())
                    sb.Append(" V ");
            }

            Console.WriteLine(sb);
            Console.ReadKey();
        }

        private static IEnumerable<bool> FunctionResultParce(string input)
        {
            var result = new List<bool>();
            foreach (var value in input)
            {
                result.Add(value == '1');
            }

            return result;
        }

        private static IEnumerable<IEnumerable<KeyValuePair<int, bool>>> GetSDNF(IEnumerable<bool> functionResult)
        {
            var result = new List<List<KeyValuePair<int, bool>>>();

            var variablesCount = (int)Math.Log(functionResult.Count(), 2);

            var dictionary = GetDictionary(variablesCount);

            var sdnfCount = 0;
            for (var index = 0; index < functionResult.Count(); index++)
            {

                var isOne = functionResult.ElementAt(index);
                if (isOne)
                {
                    result.Add(new List<KeyValuePair<int, bool>>());
                    for (var variableNumber = 0; variableNumber < dictionary.Count(); variableNumber++)
                    {
                        var queLineOfVariable = dictionary.ElementAt(variableNumber);
                        var element = queLineOfVariable.ElementAt(index);

                        var sdnfPair = new KeyValuePair<int, bool>(variableNumber, element);
                        result[sdnfCount].Add(sdnfPair);
                    }
                    sdnfCount++;
                }
            }

            return result;
        }

        private static IEnumerable<IEnumerable<bool>> GetDictionary(int variablesCount)
        {
            if (variablesCount == 0) return null;

            var result = new List<List<bool>>();
            variablesCount = Math.Abs(variablesCount);

            var numberOfRowsInTable = Math.Pow(2, variablesCount);


            var counter = 1;
            for(var index = 0; index < variablesCount; index++)
            {
                result.Add(new List<bool>());

                var blockCount = Math.Pow(2, counter);
                var membersOfblockCount =  numberOfRowsInTable / blockCount;

                var param = false;

                for (var currentBlock = 0; currentBlock < blockCount; ++currentBlock)
                {
                    for (var i = 0; i < membersOfblockCount; ++i)
                    {
                        result[index].Add(param);
                    }

                    param = !param;
                }

                ++counter;
            }

            return result;
        }

        private static IEnumerable<IEnumerable<KeyValuePair<int, bool>>> SimplifySDNF(IEnumerable<IEnumerable<KeyValuePair<int, bool>>> sdnf)

        {
            var result = new List<List<KeyValuePair<int, bool>>>();
            var havePairList = new List<int>();
            for (var currentIndex = 0; currentIndex < sdnf.Count(); currentIndex++)
            {
                var currentNode = sdnf.ElementAt(currentIndex);
                for (var nextIndex = currentIndex + 1; nextIndex < sdnf.Count(); nextIndex++)
                {
                    var nextNode = sdnf.ElementAt(nextIndex);
                    
                    var differenceList = new List<int>();
                    var isPair = true;
                    for (var elementNumber = 0; elementNumber < currentNode.Count(); elementNumber++)
                    {
                        var firstElement = currentNode.ElementAt(elementNumber);
                        var secondElement = nextNode.ElementAt(elementNumber);

                        var isEqualKey = firstElement.Key == secondElement.Key;
                        var isEqualState = firstElement.Value == secondElement.Value;
                        if (isEqualKey && !isEqualState)
                        {
                            differenceList.Add(elementNumber);
                        }
                        else if (!isEqualKey)
                        {
                            isPair = false;
                            break;
                        }
                    }

                    if (differenceList.Count == 1 && isPair)
                    {
                        var isConstainsfirst = havePairList.Contains(currentIndex);
                        if(!isConstainsfirst)
                            havePairList.Add(currentIndex);

                        var isConstainsSecond = havePairList.Contains(nextIndex);
                        if (!isConstainsSecond)
                            havePairList.Add(nextIndex);

                        var clearNode = currentNode.ToList();
                        clearNode.RemoveAt(differenceList.First());                        
                        result.Add(clearNode);
                    }
                }

            }

            var sdnfCopy = sdnf.ToList();
            for (int i = havePairList.Count - 1; i >= 0; i--)
            {
                sdnfCopy.RemoveAt(i);
            }
            foreach (var node in sdnfCopy)
            {
                result.Add(node.ToList());
            }

            var slimResult = SlimSDNF(result);
            return slimResult;
            
        }

        private static IEnumerable<IEnumerable<KeyValuePair<int, bool>>> SlimSDNF(
            IEnumerable<IEnumerable<KeyValuePair<int, bool>>> sdnf)
        {
            var result = new List<List<KeyValuePair<int, bool>>>();

            foreach (var node in sdnf)
            {
                var isConstains = result.Contains(node);
                if (!isConstains)
                {
                    result.Add(node.ToList());
                }
            }

            return result;
        }

        private static IEnumerable<IEnumerable<KeyValuePair<int, bool>>> FullSimplifySDNF(
            IEnumerable<IEnumerable<KeyValuePair<int, bool>>> sdnf)
        {

            var copyOfSdnf = sdnf.ToList();

            var isEqual = false;

            while (!isEqual)
            {
                var simplifySDNF = SimplifySDNF(copyOfSdnf);
                isEqual = CompareSDNF(simplifySDNF, copyOfSdnf);

                if (!isEqual)
                    copyOfSdnf = simplifySDNF.ToList();
            }
            return copyOfSdnf;
        }

        private static bool CompareSDNF(IEnumerable<IEnumerable<KeyValuePair<int, bool>>> first,
            IEnumerable<IEnumerable<KeyValuePair<int, bool>>> second)
        {
            var isEqualSize = first.Count() == second.Count();

            if (!isEqualSize)
                return false;

            for (var index = 0; index < first.Count(); index++)
            {
                var isEqualNodeSize = first.ElementAt(index).Count() == second.ElementAt(index).Count();
                if (!isEqualNodeSize)
                    return false;

                var isEqualNode = first.ElementAt(index).Equals(second.ElementAt(index));
                if (isEqualNode)
                    return false;

            }

            return true;
        }
    }
}
