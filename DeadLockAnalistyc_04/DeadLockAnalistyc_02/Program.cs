using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Glee.GraphViewerGdi;

namespace DeadLockAnalistyc_04
{
    class Program
    {
        private static readonly Regex InitProcess = new Regex(@"([a-z]+) \(([0-9| ]*)\) \(([0-9| ]*)\)");
        private static readonly Regex InitResource = new Regex(@"init ([0-9]+)");
        private static readonly Regex GiveProcRes = new Regex(@"([a-z]+) take ([0-9]+)");
        private static readonly Regex RequestProcRes = new Regex(@"([a-z]+) request ([0-9]+)");
        private static readonly Regex ProcFreeRes = new Regex(@"([a-z]+) free ([0-9]+)");
        private static readonly Regex CancelRecuest = new Regex(@"([a-z]+) cancel ([0-9]+)");
        private static readonly char[] Separators = { ' ' }; 



        static void Main(string[] args)
        {
            Console.WriteLine("File Instructions\n" +
                              "1) init [short]           \\\\- init resource in syste\n" +
                              "2) [name] ([take id] [take id] ... ...) ([request id] [reques id] ... ...) " +
                              " \\\\ init proc\n" +
                              "3) [name] take [resId]    \\\\take resource\n" +
                              "4) [name] request [resId] \\\\request resource\n" +
                              "5) [name] free [resId]    \\\\free resource\n" +
                              "6) [name] cancel [resId]  \\\\remove request on resource\n" +
                              "\n"
                              );

            var mySystem = new MySystem();
            try
            {
                var sr = new StreamReader(@"input.txt");
                
                var strInFile = sr.ReadLine();
                var lineCounter = 1;
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
                            var takeResSrtrs = match.Groups[2].ToString()
                                .Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                            var requestResStrs = match.Groups[3].ToString()
                                .Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                            mySystem.InitProcess(name, takeResSrtrs, requestResStrs);
                            Console.WriteLine($"Process with name [{name}] added in system successfuly");
                        }
                        else if (GiveProcRes.IsMatch(strInFile))
                        {
                            var match = GiveProcRes.Match(strInFile);
                            var procName = match.Groups[1].ToString();
                            var resIdStr = match.Groups[2].ToString();

                            mySystem.ProcGetRes(procName, resIdStr);
                            Console.WriteLine(
                                $"Process with name [{procName}] took resource with id [{resIdStr}] successfuly");
                        }
                        else if (RequestProcRes.IsMatch(strInFile))
                        {
                            var match = RequestProcRes.Match(strInFile);
                            var procName = match.Groups[1].ToString();
                            var resIdStr = match.Groups[2].ToString();

                            mySystem.RequestGetRes(procName, resIdStr);
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
                        else if (CancelRecuest.IsMatch(strInFile))
                        {
                            var match = CancelRecuest.Match(strInFile);
                            var procName = match.Groups[1].ToString();
                            var resIdStr = match.Groups[2].ToString();

                            mySystem.CanselRequest(procName, resIdStr);
                            Console.WriteLine(
                                $"Process with name [{procName}] canceled request on resource with id [{resIdStr}] successfuly");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    ++lineCounter;
                    strInFile = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var form = new Form();
            var viewer = new GViewer();
            var graph = mySystem.GetGraph("first");

            viewer.Graph = graph;
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();

            form.ShowDialog();

            Console.ReadKey();
        }
    }
}
