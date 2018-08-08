using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _10_01_poemConverter
{
    // модификатор доступа ? наследуешься ?
    public sealed class Poem
    {
        /// <summary>
        /// Правда все не readonly ?
        /// </summary>
        private string _poem;
        private readonly StringBuilder _processedPoem;
        private string _dictionaryWay;
        private string _poemPath;

        // в конструктор
        private static char[] _vowels;
        // туда же
        private readonly SortedDictionary<int, List<string>> _dictionary;


        //Constructor под филдами, т.е. выше свойств
        public Poem(string poemPath, string dictionaryWay, string outputPath)
        {
            _processedPoem = new StringBuilder();
            _vowels = new []{ 'а', 'о', 'и','е','ё','э','ы','у','ю','я'};
            _dictionary = new SortedDictionary<int, List<string>>();
            PoemPath = poemPath;
            DictionaryWay = dictionaryWay;
            OutputPath = outputPath;

            Process();
        }

        // properties
        public string ProcPoem => _processedPoem.ToString();
        public string DictionaryWay
        {
            get => _dictionaryWay;

            set
            {
                if (File.Exists(value))
                {
                    _dictionaryWay = value;

                    if (_dictionary.Count != 0)
                        _dictionary.Clear();

                    StreamReader sr = File.OpenText(value);

                    string tmpstr;
                    // сделать условия понятными
                    while ((tmpstr = sr.ReadLine()) != null)
                    {
                        tmpstr = tmpstr.Trim();
                        int count = VolwesConsist(tmpstr);

                        if (!_dictionary.ContainsKey(count))
                        {
                            _dictionary[count] = new List<string>();
                            _dictionary[count].Add(tmpstr);
                        }
                        else
                        {
                            _dictionary[count].Add(tmpstr);
                        }
                      
                    }
                    sr.Close();

                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
        }
        public string OutputPath { get; set; }

        public string PoemPath
        {
            get => _poemPath;
       
            set
            {
                if (File.Exists(value))
                {
                    StreamReader sr = File.OpenText(value);
                    _poem = sr.ReadToEnd();
                    _poemPath = value;
                    sr.Close();
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
        }

        // вызывается из к-тора -> private
        private string Process()
        {
            string[] splitPoem = _poem.Split(new char[] {' ', '\n', '\r', '\t', '-'}, StringSplitOptions.RemoveEmptyEntries);
            StreamWriter sw = File.CreateText(OutputPath);

            string[] newPoem = new string[splitPoem.Length];

            int counter = 0;
            foreach (string str in splitPoem)
            {
                string procstr = str.Trim('.', ' ', '?', '!', ':', ',', ';');
                int volwescount = VolwesConsist(procstr);

                if ((volwescount <= 1 && procstr.Length <= 4) || (volwescount == 2 && procstr.Length <= 3))
                {
                    newPoem[counter] = procstr;
                }
                else
                {
#if DEBUG
                    Console.WriteLine("finding word -------- {0}", procstr);
#endif
                    try
                    {
                        newPoem[counter] = WordFromDictionaryV2(volwescount, procstr.Substring(procstr.Length - 3, 3));
                    }
                    catch (Exception e)
                    {
                        newPoem[counter] = procstr;
                    }

                }

                ++counter;
            }

            counter = 0;
            bool word = false;
            foreach (char c in _poem)
            {
                if (!char.IsLetter(c))
                {
                    word = false;
                    _processedPoem.Append(c);
                }
                else if (word == false)
                {
                    word = true;
                    
        
                    if (Char.IsUpper(c) && Char.IsLower(newPoem[counter][0]))
                    {
                        
                        _processedPoem.Append(Char.ToUpper(newPoem[counter][0]) + newPoem[counter].Substring(1, newPoem[counter].Length - 1));
                    }
                    else if (Char.IsLower(c) && Char.IsUpper(newPoem[counter][0]))
                    {
                        _processedPoem.Append(Char.ToLower(newPoem[counter][0]) + newPoem[counter].Substring(1, newPoem[counter].Length - 1));
                    }
                    else
                    {
                        _processedPoem.Append(newPoem[counter]);
                    }
                    ++counter;
                }
            }

            sw.WriteLine(_processedPoem);
            sw.Close();
            return _processedPoem.ToString();
        }

        private string WordFromDictionaryV2(int vowelsCount, string ending)
        {
            // var`ы
            var listWithWords = _dictionary[vowelsCount];

            var probablyCount = listWithWords.Count;

            var satisfyingWords = new string[probablyCount];
            var lessSatisfyingWords = new string[probablyCount];
            var satisfyingWordsCounter = 0;
            var lessSatisfyingWordsCounter = 0;



            foreach (string wordFromDictionary in listWithWords)
            {
                var isIdenticalSmallEnding = wordFromDictionary[wordFromDictionary.Length - 1] == ending[ending.Length - 1];
                var wordIsLongerThenEnding = wordFromDictionary.Length >= ending.Length;

                // условие
                if (wordIsLongerThenEnding && isIdenticalSmallEnding)
                {
                    lessSatisfyingWords[lessSatisfyingWordsCounter++] = wordFromDictionary;

                    int endingIndex = wordFromDictionary.Length - ending.Length;
#if DEBUG
                    Console.WriteLine("need = {0}       have = {1}", ending, wordFromDictionary.Substring(endingIndex, ending.Length));
#endif
                    var isIdenticalBigEnding = wordFromDictionary.Substring(endingIndex, ending.Length) == ending;

                    if (isIdenticalBigEnding)
                    {
                        satisfyingWords[satisfyingWordsCounter++] = wordFromDictionary;
                    }

                    var isOutofRange = lessSatisfyingWordsCounter >= probablyCount
                                       || satisfyingWordsCounter >= probablyCount;

                    if (isOutofRange)
                        break;

                }
            }

            Random rand = new Random();


            if (satisfyingWordsCounter != 0)
            {
                return satisfyingWords[rand.Next(0, satisfyingWordsCounter - 1)];
            }
            else if (lessSatisfyingWordsCounter != 0)
            {
                return lessSatisfyingWords[rand.Next(0, lessSatisfyingWordsCounter - 1)];
            }
            else
            {
                throw new Exception("Word wasn't find");
            }
        }
        
        private static int VolwesConsist(string word)
        {
            int counter = 0;

            foreach (char c in word)
            {
                if (_vowels.Contains(c))
                {
                    ++counter;
                }
            }

            return counter;
        }

    }
}
