using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05_01_strChange
{
    public static class StringEditor
    {
        public static string SortOutLastLetters(string[] inputArr)
        {
            for (var i = 0; i < inputArr.Length; i++)
            {
                for (var j = 0; j < inputArr.Length; j++)
                    if (inputArr[i][0] < inputArr[j][0])
                    {
                        var tmpstr = inputArr[i];
                        inputArr[i] = inputArr[j];
                        inputArr[j] = tmpstr;
                    }
            }

            var lastLettersWordBuilder = new StringBuilder();
            foreach (var word in inputArr)
            {
                lastLettersWordBuilder.Append(word[word.Length - 1]);
            }

            return lastLettersWordBuilder.ToString();
        }

        public static string UpFirstDownLast(string inputStr)
        {
            var separators = new[] {' '};
            var sequence = inputStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return UpFirstDownLast(sequence);
        }

        public static string UpFirstDownLast(string[] inputArr)
        {
            var sb = new StringBuilder[inputArr.Length];

            for (var i = 0; i < sb.Length; i++)
            {
                sb[i] = new StringBuilder();
                sb[i].Append(inputArr[i]);
                sb[i][0] = char.ToUpper(sb[i][0]);

                var index = sb[i].Length - 1;
                sb[i][index] = char.ToLower(sb[i][index]);
            }

            return StrArrToString(sb);
        }

        public static int MeetCount(string inputStr, string searchingWord)
        {
            var separators = new[] { ' ' };
            var sequence = inputStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return MeetCount(sequence, searchingWord);
        }

        public static int MeetCount(string[] inputStrArr, string searchingWord)
        {
            var counter = inputStrArr.Count(tmpstr => tmpstr == searchingWord);
            return counter;
        }

        public static string ReplacePenultimateWord(string inputStr, string replacingWord)
        {
            var separators = new[] { ' ' };
            var sequence = inputStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                return ReplacePenultimateWord(sequence, replacingWord);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static string ReplacePenultimateWord(string[] inputStrArr, string replacingWord)
        {
            if (inputStrArr.Length - 2 >= 0)
                inputStrArr[inputStrArr.Length - 2] = replacingWord;
            else
                throw new Exception("#Error: Еhe sentence has less than 2 words");

            return StrArrToString(inputStrArr);
        }

        public static string ReturnUpWord(string[] inputstr, int number)
        {
            var outputStr = "";
            foreach (var tmpstr in inputstr)
            {
                if (char.IsUpper(tmpstr[0]))
                    --number;
                if (number == 0)
                {
                    outputStr = tmpstr;
                }
            }

            return outputStr;
        }

        public static string ReturnUpWord(string inputStr, int number)
        {
            var separators = new[] { ' ' };
            var sequence = inputStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return ReturnUpWord(sequence, number);
            
        }   






        private static string StrArrToString<T>(IReadOnlyCollection<T> strArr)
        {
            var sb = new StringBuilder();
            
            if (strArr.Count != 0)
            {
                foreach (var str in strArr)
                {
                    sb.Append(str);
                    sb.Append(' ');
                }

                var lastIndex = strArr.Count - 1;
                sb.Remove(lastIndex, 1);
            }

            return sb.ToString();
        }
    }
}
