using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _11_03_DirectoryWalk
{
    class Program
    {



        public enum FileMenuActions
        {
            Exit,
            Open,
            Archive,
            Stats
        };

        
        [STAThread]
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
                var foundfileNames = GetFilesByOption(mainAction);

                if (!foundfileNames.Any())
                {
                    Console.WriteLine("File(s) not found");
                    break;
                }

                var counter = 1;
                var sb = new StringBuilder();
                foreach (var fileName in foundfileNames)
                {
                    sb.Append(counter.ToString() + ". " + fileName.ToString() + "\n");
                    ++counter;
                }
                Console.WriteLine(sb);

                /////////////////////////////////////
                while (true)
                {
                    PrintActionOnFileMenu();
                    var isWrongFileMenuComand = true;
                    var fileMenuAction = FileMenuActions.Exit;

                    while (isWrongFileMenuComand)
                    {
                        var fileMenuComandStr = Console.ReadLine();

                        try
                        {
                            fileMenuAction = GetFailMenuAction(fileMenuComandStr);
                            isWrongFileMenuComand = false;
                        }
                        catch (Exception e)
                        {
                            Console.Write($"#Error: {e.Message}.Try again: ");
                        }
                    }

                    if (fileMenuAction == FileMenuActions.Exit)
                    {
                        Console.WriteLine("----------end file menu----------");
                        break;
                    }

                    Console.WriteLine($"Chose file number [1-{foundfileNames.Count()}]: ");
                    var parseResult = false;
                    var index = 0;
                    do
                    {
                        var inputNumberStr = Console.ReadLine();
                        parseResult = int.TryParse(inputNumberStr, out index);
                        var inRange = index >= 1 && index <= foundfileNames.Count();

                        if (!parseResult)
                        {
                            Console.Write("#Error input. Try again: ");
                        }
                        else if (!inRange)
                        {
                            Console.Write("#Error: Number out of Range. Try again: ");
                            parseResult = false;
                        }

                    } while (!parseResult);

                    var foundfileNamesList = foundfileNames.ToList();
                    ActionOnFile(fileMenuAction, foundfileNamesList[index - 1]);
                }

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

        public static void PrintActionOnFileMenu()
        {
            Console.Write("--------Action on file Menu ----------\n" +
                          "1. Open;\n" +
                          "2. Archive;\n" +
                          "3. Words stat;\n" +
                          "0. Exit\n" +
                          "You enter: "
            );
        }

        public static IEnumerable<FileInfo> GetFilesByOption(MainMenuActions action)
        {

            /*if (!ComandArr.Contains(action))
                throw new Exception("#Error: unknown comand");
                */

            Console.Write("\nEnter searching string or pattern (@ - 1 letter, ? - many letter):  ");
            var searcingStr = Console.ReadLine();

            IEnumerable<FileInfo> foundFiList;
            Regex reg;

            Console.WriteLine("\nLoading... Pls wait...\n");

            switch (action)
            {
                case MainMenuActions.ByName:
                    reg = GetRegex(searcingStr, true);
                    foundFiList = FileSearch(reg);
                    break;

                case MainMenuActions.ByPatternName:
                    reg = GetRegex(searcingStr);
                    foundFiList = FileSearch(reg);
                    break;

                case MainMenuActions.ByStringInFile:
                    reg = GetRegex(searcingStr, true);
                    foundFiList = FileSearch(reg, true);
                    break;

                case MainMenuActions.ByPatternStringInFile:
                    reg = GetRegex(searcingStr);
                    foundFiList = FileSearch(reg, true);
                    break;

                default:
                    foundFiList = null;
                    break;

            }

            return foundFiList;
        }

        public static void ActionOnFile(FileMenuActions action, FileInfo fileInfo)
        {

            switch (action)
            {
                case FileMenuActions.Open:
                    OpenFile(fileInfo);
                    Console.WriteLine("Open - OK");
                    break;

                case FileMenuActions.Archive:
                    ArchiveFile(fileInfo);
                    Console.WriteLine("Archiv - OK");
                    break;

                case FileMenuActions.Stats:
                    WordInFileStatic(fileInfo);
                    Console.WriteLine("stats - OK");
                    break;
            }

            Console.WriteLine();
        }

        private static void OpenFile(FileSystemInfo file)
        {
            var processStartInfo = new ProcessStartInfo(file.FullName)
            {
                Arguments = Path.GetFileName(file.FullName),
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(file.FullName) ?? throw new Exception("#Error: Path is lost"),
                Verb = "OPEN"
            };
            Process.Start(processStartInfo);
        }

        private static void WordInFileStatic(FileInfo file)
        {
            var sr = file.OpenText();
            var wordsStatsDict = new Dictionary<string, int>();

            var separators = new[] {' ', ',', '.', ':', ';', '?', '!', '(', ')'};

            var strFromFile = sr.ReadLine();
            while (strFromFile != null)
            {
                var splitedStr = strFromFile.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in splitedStr)
                {
                    if (wordsStatsDict.ContainsKey(word))
                    {
                        ++wordsStatsDict[word];
                    }
                    else
                    {
                        wordsStatsDict[word] = 1;
                    }
                }
                strFromFile = sr.ReadLine();
            }
            sr.Dispose();


            var sb = new StringBuilder();
            foreach (var pair in wordsStatsDict)
            {
                sb.Append(pair);
                sb.Append("  ");
            }
            Console.WriteLine(sb);

        }


        [STAThread]
        private static void ArchiveFile(FileInfo file)
        {

            try
            {
                var browserDialog = new FolderBrowserDialog();
                
                if (browserDialog.ShowDialog() != DialogResult.OK) return;
                var filePath = browserDialog.SelectedPath;

                using (var originalFileStream = file.OpenRead())
                {
                    var isReadyFile = (File.GetAttributes(file.FullName) &
                                       FileAttributes.Hidden) != FileAttributes.Hidden & file.Extension != ".gz";
                    if (isReadyFile)
                    {
                        using (var compressedFileStream = File.Create(filePath + "\\" + file.Name + ".gz"))
                        {
                            using (var compressionStream = new GZipStream(compressedFileStream,
                                CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);

                            }
                        }

                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

        private static IEnumerable<FileInfo> FileSearch(Regex reg, bool inFile = false)
        {
            var fiList = new List<FileInfo>();
            var dirs = new Stack<string>();
            var drives = Environment.GetLogicalDrives();

            foreach (var dr in drives)
            {
                var di = new DriveInfo(dr);

                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                var rootDir = di.RootDirectory;

                dirs.Push(rootDir.ToString());

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
                        try
                        {
                            var fi = new FileInfo(file);

                            var isTxt = fi.Name.EndsWith(".txt");
                            if (inFile && isTxt)
                            {

                                var sr = fi.OpenText();
                                var strFromFile = sr.ReadLine();

                                while (strFromFile != null)
                                {
                                    var wasFoundPattern = reg.IsMatch(strFromFile);

                                    if (wasFoundPattern)
                                    {
                                        fiList.Add(fi);
                                        sr.Dispose();
                                        break;
                                    }

                                    strFromFile = sr.ReadLine();
                                }
                            }
                            else
                            {
                                var wasFoundPatternInName = reg.IsMatch(fi.Name);

                                if (wasFoundPatternInName)
                                {
                                    fiList.Add(fi);
                                }
                            }

                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    foreach (var str in subDirs)
                        dirs.Push(str);
                }

                Console.WriteLine($"Logical drive {dr} - complete");
            }
            return fiList;
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

        private static FileMenuActions GetFailMenuAction(string inputComand)
        {
            switch (inputComand)
            {
                case "0":
                    return FileMenuActions.Exit;

                case "1":
                    return FileMenuActions.Open;

                case "2":
                    return FileMenuActions.Archive;

                case "3":
                    return FileMenuActions.Stats;

                default:
                    throw new Exception("Invalid Comand");
            }
        }
    }

}
