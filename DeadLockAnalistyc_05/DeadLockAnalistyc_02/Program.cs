using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Glee.GraphViewerGdi;

namespace DeadLockAnalistyc_05
{
    public sealed class Program
    {
        private static readonly Regex InitProcess = new Regex(@"([a-z]+) \(([0-9| ]+)\)");
        private static readonly Regex InitResource = new Regex(@"init ([0-9]+)");
        private static readonly Regex GiveProcRes = new Regex(@"([a-z]+) request ([0-9]+)");
        private static readonly Regex ProcFreeRes = new Regex(@"([a-z]+) free ([0-9]+)");
        private static readonly char[] Separators = { ' ' };


        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("File Instructions\n" +
                              "1) init [short]           \\\\- init resource in syste\n" +
                              "2) [name] ([take id] [take id] ... ...)" +
                              " \\\\ init proc\n" +
                              "3) [name] request [resId] \\\\request resource\n" +
                              "4) [name] free [resId]    \\\\free resource\n" +
                              "\n"
            );

            var mySystem = new MySystem();
            var sr = new StreamReader(@"input.txt");

            var strInFile = sr.ReadLine();
            while (strInFile != null)
            {
                try
                {
                    if (InitResource.IsMatch(strInFile))
                    {
                        var match = InitResource.Match(strInFile);
                        mySystem.InitResource(match.Groups[1].ToString());
                        Console.WriteLine($"Resource with id [{match.Groups[1]}] added in system successfuly");

                    }
                    else if (InitProcess.IsMatch(strInFile))
                    {
                        var match = InitProcess.Match(strInFile);
                        var name = match.Groups[1].ToString();
                        var resSrtrs = match.Groups[2].ToString()
                            .Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        mySystem.InitProcess(name, resSrtrs);
                        Console.WriteLine($"Process with name [{name}] added with resources [{match.Groups[2]}] in system successfuly");
                    }
                    else if (GiveProcRes.IsMatch(strInFile))
                    {
                        var match = GiveProcRes.Match(strInFile);
                        var procName = match.Groups[1].ToString();
                        var resIdStr = match.Groups[2].ToString();

                        mySystem.ProcGetRes(procName, resIdStr);
                        Console.WriteLine(
                            $"Process with name [{procName}] request resource with id [{resIdStr}] successfuly");
                    }
                    else if (ProcFreeRes.IsMatch(strInFile))
                    {
                        var match = ProcFreeRes.Match(strInFile);
                        var procName = match.Groups[1].ToString();
                        var resIdStr = match.Groups[2].ToString();

                        mySystem.ProcFreeRes(procName, resIdStr);
                        Console.WriteLine(
                            $"Process with name [{procName}] free resource with id [{resIdStr}] successfuly");
                    }
                }
                catch (DeadlockException dex)
                {
                    Console.WriteLine(dex.Message);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                strInFile = sr.ReadLine();
            }


            mySystem.ShowGraph();

            Console.ReadKey();
        }
    }
}
