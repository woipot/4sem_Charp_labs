using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _11_03_DirectoryWalk
{
    class Program
    {
        public enum MainMenuActions
        {
            Exit,
            ByName,
            ByPatternName,
            ByStringInFile,
            ByPatternStringInFile
        };

        static void Main(string[] args)
        {
            while (true)
            {
                ///////////////////////
                PrintMainMenu();
                var isWrongComand = true;

                var mainAction = MainMenuActions.Exit;
                while (isWrongComand)
                {
                    var comandStr = Console.ReadLine();

                    try
                    {
                        mainAction = GetMainMenuAction(comandStr);
                        isWrongComand = false;
                    }
                    catch (Exception e)
                    {
                        Console.Write($"#Error: {e.Message}.Try again: ");
                    }
                }

                if (mainAction == MainMenuActions.Exit)
                    break;
                ////////////////////////////////////////
               PrintFilesByOption(mainAction);

               
            }

            Console.WriteLine("Enter eny key to exit");
            Console.ReadKey();
        }

        public static void PrintMainMenu()
        {
            Console.Write(
                "======Main Menu======\n" +
                "Searching by:\n" +
                "1. Name;\n" +
                "2. Pattern Name;\n" +
                "3. String in file;\n" +
                "4. Pattern string in file;\n" +
                "0. To close program;\n" +
                "You enter: ");
        }

    

        public static void PrintFilesByOption(MainMenuActions action)
        {

            /*if (!ComandArr.Contains(action))
                throw new Exception("#Error: unknown comand");
                */

            string root;
            while(true)
            {
                Console.Write("Enter Root directory: ");
                root = Console.ReadLine();
                if (!Directory.Exists(root))
                {
                    Console.WriteLine("#Error: Directory not exist. Try again... ");
                }
                else
                {
                    break;
                }
            }

            Console.Write("\nEnter searching string or pattern (@ - 1 letter, ? - many letter):  ");
            var searcingStr = Console.ReadLine();

            Regex reg;

            Console.WriteLine("\nLoading... Pls wait...\n");

            switch (action)
            {
                case MainMenuActions.ByName:
                    reg = GetRegex(searcingStr, true);
                    FileSearch(root, reg);
                    break;

                case MainMenuActions.ByPatternName:
                    reg = GetRegex(searcingStr);
                    FileSearch(root, reg);
                    break;

                case MainMenuActions.ByStringInFile:
                    reg = GetRegex(searcingStr, true);
                    FileSearch(root, reg, true);
                    break;

                case MainMenuActions.ByPatternStringInFile:
                    reg = GetRegex(searcingStr);
                    FileSearch(root, reg, true);
                    break;

            }

        }


        private static Regex GetRegex(string inputStr, bool isExactMath = false)
        {
            var sb = new StringBuilder(inputStr);

            sb = sb.Replace(".", "\\.");
            sb = sb.Replace("@", ".");
            sb = sb.Replace("?", "[^ ]");
            if (!isExactMath) return new Regex(sb.ToString());

            sb = sb.Insert(0, "^");
            sb.Append("$");
            return new Regex(sb.ToString());
        }

        private static void FileSearch(string root, Regex reg, bool inFile = false)
        {
            var taskList = new List<Task<KeyValuePair<string, IEnumerable<KeyValuePair<int, string>>>>>();



            var fiList = new List<FileInfo>();
            var dirs = new Stack<string>();

            if (!Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                var currentDir = dirs.Pop();
                string[] subDirs;
                string[] files;
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                    files = Directory.GetFiles(currentDir);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var file in files)
                {
                    var fi = new FileInfo(file);

                    var isTxt = fi.Name.EndsWith(".txt");
                    var isRtf = fi.Name.EndsWith(".rtf");
                    if (inFile && (isTxt || isRtf ))
                    {
                        var newTask = new Task<KeyValuePair<string, IEnumerable<KeyValuePair<int, string>>>>
                            (() => SearchInFile(fi, reg));
                        newTask.Start();
                        taskList.Add(newTask);
                       
                    }
                    else if (!inFile)
                    {
                        var wasFoundPatternInName = reg.IsMatch(fi.Name);

                        if (wasFoundPatternInName)
                        {
                            fiList.Add(fi);
                        }
                    }

                }

                foreach (var str in subDirs)
                    dirs.Push(str);
            }

            if (inFile)
            {
                var resultList = new List<KeyValuePair<string, IEnumerable<KeyValuePair<int, string>>>>();
                foreach (var task in taskList)
                {
                    var taskRes = task.Result;
                    if (taskRes.Value.Any())
                        resultList.Add(taskRes);
                }

                var strw = new StreamWriter("out.txt");
                foreach (var keyValue in resultList)
                {
                    Console.WriteLine(keyValue.Key);
                    foreach (var countAndStr in keyValue.Value)
                    {
                        Console.WriteLine($"{countAndStr.Key} ---  {countAndStr.Value}");
                    }
                    Console.Write("\n\n");

                    strw.WriteLine(keyValue.Key);
                    foreach (var countAndStr in keyValue.Value)
                    {
                        strw.WriteLine($"{countAndStr.Key} ---  {countAndStr.Value}");
                    }

                    

                }
                strw.Dispose();

            }
            else
            {


                if (!fiList.Any())
                {
                    Console.WriteLine("File(s) not found");
                }

                var counter = 1;
                var sb = new StringBuilder();
                foreach (var fileName in fiList)
                {
                    sb.Append(counter.ToString() + ". " + fileName.ToString() + "\n");
                    ++counter;
                }
                Console.WriteLine(sb);
                
            }

        }

        private static MainMenuActions GetMainMenuAction(string inputComand)
        {
            switch (inputComand)
            {
                case "0":
                    return MainMenuActions.Exit;

                case "1":
                    return MainMenuActions.ByName;

                case "2":
                    return MainMenuActions.ByPatternName;

                case "3":
                    return MainMenuActions.ByStringInFile;

                case "4":
                    return MainMenuActions.ByPatternStringInFile;

                default:
                    throw new Exception("Invalid Comand");
            }
        }


        private static KeyValuePair<string, IEnumerable<KeyValuePair<int, string>>> SearchInFile(FileInfo fi, Regex reg)
        {
            var resultList = new List<KeyValuePair<int, string>>();

            var sr = fi.OpenText();
            var strFromFile = sr.ReadLine();
            var strCounter = 1;

            while (strFromFile != null)
            {
                var wasFoundPattern = reg.IsMatch(strFromFile);

                if (wasFoundPattern)
                {
                    var foundPair = new KeyValuePair<int, string>(strCounter, strFromFile);
                    resultList.Add(foundPair);
                }

                ++strCounter;
                strFromFile = sr.ReadLine();
            }

            sr.Dispose();
            var res = new KeyValuePair<string, IEnumerable<KeyValuePair<int, string>>>(fi.Name, resultList);
            return res;
        }

    }

}
