using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeadLockAnalistyc_05;
using Microsoft.Glee.Drawing;
using Microsoft.Glee.GraphViewerGdi;

namespace DeadLockAnalistyc_05
{
    public sealed class MySystem
    {
        private readonly ProcessesController _processesController;
        private readonly ResourcesAllocator  _resourcesAllocator;



        #region Constructors

        public MySystem()
        {
            _resourcesAllocator  = new ResourcesAllocator();
            _processesController = new ProcessesController();

            _resourcesAllocator.ResourceReliaseEvent += _processesController.CompleteRequest;
            _processesController.FreeResourceEvent += (obj, id) => _resourcesAllocator.DeleteLink(id);
        }

        #endregion



        #region Public methods

        public void InitResource(short resourceId)
        {
            var isReally = _resourcesAllocator.IsReallyResource(resourceId);
            if (isReally)
            {
                throw new Exception($"#Error: resource [{resourceId}] already constaint in system");
            }

            _resourcesAllocator.AddResource(resourceId);
        }

        public void InitResource(string resourceIdStr)
        {
            var isShort = short.TryParse(resourceIdStr, out var resourceId);

            if (!isShort)
            {
                throw new Exception($"#Error: Uncorrect resource id [{resourceIdStr}]");
            }

            InitResource(resourceId);
        }



        public void InitProcess(string procName, IEnumerable<short> resources)
        {
            var inSystem = _processesController.IsReallyProcess(procName);
            if(inSystem)
                throw new Exception($"#Error: Process with a same name [{procName}] is already used in system");

            var newProcess = new Process(procName);
            _processesController.AddProcess(newProcess);

            foreach (var resourceId in resources)
            {
                ProcGetRes(procName, resourceId);
            }

        }

        public void InitProcess(string procName, IEnumerable<string> resourcesStrArr)
        {
            var resourcesList = new List<short>();
            foreach (var resStr in resourcesStrArr)
            {
                var isShort = short.TryParse(resStr, out var res);

                if (!isShort)
                {
                    throw new Exception($"#Error: Wrong argument in init process [{procName}] in [{resStr}]");
                }
                resourcesList.Add(res);
            }


            InitProcess(procName, resourcesList);
        }



        public void ProcGetRes(string procName, short resourceId)
        {
            var isReallyResource = _resourcesAllocator.IsReallyResource(resourceId);
            var isReallyProcess = _processesController.IsReallyProcess(procName);

            if (isReallyProcess && isReallyResource)
            {
                var isFreeResource = _resourcesAllocator.IsFreeResource(resourceId);
                if (isFreeResource)
                {
                    _resourcesAllocator.MakeLink(resourceId, procName);
                    _processesController.AddAvailableRes(procName, resourceId);
                }
                else
                {
                    _processesController.AddRequestRes(procName, resourceId);
                }
            }
            else
            {
                throw new Exception($"#Error: It is Imposible to give the process [{procName}] resource [{resourceId}]. Resource or Process is absent in system");
            }


            try
            {
                SafeCheck();
            }
            catch (Exception e)
            {
                var additionMessage = $"#Error: Init process [{procName}] goes to deadlock:\n";
                throw new DeadlockException(additionMessage + e.Message);
            }

        }

        public void ProcGetRes(string procName, string resourceId)
        {
            var isShort = short.TryParse(resourceId, out var res);

            if (!isShort)
            {
                throw new Exception($"#Error: Impossible to get process [{procName}] resource. Wrong argument in resource id [{resourceId}]");
            }


            ProcGetRes(procName, res);
        }



        public void ProcFreeRes(string procName, short resourceId)
        {
            var isReallyResource = _resourcesAllocator.IsReallyResource(resourceId);
            if (!isReallyResource)
            {
                throw new Exception($"#Error: Process [{procName}] can't free a resource. Resource is absent in system");
            }

            var isReallyProcess = _processesController.IsReallyProcess(procName);
            if (!isReallyProcess)
                throw new Exception(
                    $"#Error: Process [{procName}] can't free a resource. Process doesn't constains in system");

            _processesController.FreeResource(procName, resourceId);

            try
            {
                SafeCheck();
            }
            catch (Exception e)
            {
                var additionMessage = $"#Error: Process [{procName}] free resource [{resourceId}], This action goes to deadlock:\n";
                throw new DeadlockException(additionMessage + e.Message);
            }

        }

        public void ProcFreeRes(string procName, string resourceId)
        {
            var isShort = short.TryParse(resourceId, out var res);

            if (!isShort)
            {
                throw new Exception($"#Error: Process [{procName}] can't free a resource. Wrong argument");
            }

            ProcFreeRes(procName, res);
        }



        public void ShowGraph()
        {
            var form = new Form();
            var viewer = new GViewer();
            var graph = GetGraph("first");

            viewer.Graph = graph;
            form.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();

            form.ShowDialog();
        }
        #endregion



        #region Private Methods

        private static bool IsRepeat<TObj>(IEnumerable<TObj> checkArr)
        {
            var analysticalList = new List<TObj>();
           

            foreach (var token in checkArr)
            {
                var isConstains = analysticalList.Contains(token);
                if (isConstains)
                    return true;

                analysticalList.Add(token);
            }

            return false;
        }


        private void SafeCheck()
        {

            var processes = _processesController.Processes;
            var resources = _resourcesAllocator.Resources;

            var linksDict = new SortedDictionary<string, int>();
            foreach (var procPair in processes)
            {
                var outLinksCount = procPair.Value.NecessaryRes.Count;
                linksDict[procPair.Key] = outLinksCount;
            }

            foreach (var res in resources)
            {
                var resName = res.Key.ToString();

                linksDict[resName] = (res.Value != null) ? 1 : 0;
            }

            var nodes = linksDict.Keys.ToList();
            var analistycStack = new Stack<string>();

            foreach (var startingNode in nodes)
            {
                var currentNode = startingNode;
                analistycStack.Push(currentNode);

                while (linksDict[startingNode] != 0 || analistycStack.Count > 1)
                {
                    var outLinkNumber = linksDict[currentNode];
                    if (outLinkNumber != 0)
                    {
                        
                        var isProcess = _processesController.IsReallyProcess(currentNode);
                        string nextNode; 
                        if (isProcess)
                        {
                            nextNode = processes[currentNode].NecessaryRes[outLinkNumber - 1].ToString();
                        }
                        else
                        {
                            short.TryParse(currentNode, out var resid);
                            nextNode = resources[resid];
                        }
                        linksDict[currentNode] -= 1; 

                        analistycStack.Push(nextNode);
                        currentNode = nextNode;
                        var isCycle = IsRepeat(analistycStack);
                        if (isCycle)
                        {
                            var message = GetMeassageFromDeadlock(analistycStack);
                            throw new DeadlockException(message);
                        }
                    }
                    else
                    {
                        analistycStack.Pop();
                        currentNode = analistycStack.Peek();
                    }

                }
                analistycStack.Clear();

            }


        }


        private string GetMeassageFromDeadlock(IEnumerable<string> analystic)
        {
            var sb = new StringBuilder();
            var inDeadlock = InDeadlock(analystic);


            var proceses = new List<string>();
            var resources = new List<string>();

            foreach (var token in inDeadlock)
            {
                var isProcess = _processesController.IsReallyProcess(token);
                if (isProcess)
                {
                    proceses.Add(token);
                }
                else
                {
                    resources.Add(token);
                }
            }

            proceses.Sort();
            resources.Sort();

            sb.Append("{\nProcess in deadlock:\n");
            var counter = 1;
            foreach (var process in proceses)
            {
                sb.Append("\t" + counter + ") " + process + "\n");
                ++counter;
            }

            sb.Append("Resources in deadLock:\n");
            counter = 1;
            foreach (var resName in resources)
            {
                sb.Append("\t" + counter + ") " + resName + "\n");
                ++counter;
            }

            sb.Append("}");

            return sb.ToString();
        }


        private IEnumerable<string> InDeadlock(IEnumerable<string> analystic)
        {
            var inDeadLock = new List<string>();


            var processes = _processesController.Processes;


            var starterPackList = new List<string>();
            foreach (var token in analystic)
            {
                var isProcess = _processesController.IsReallyProcess(token);
                if (isProcess)
                {
                    var process = processes[token];
                    var processResources = process.AvailableRes.Select(resourceId => resourceId.ToString());

                    foreach (var resource in processResources)
                    {
                        var isConstains = starterPackList.Contains(resource);
                        if(!isConstains)
                            starterPackList.Add(resource);
                    }
                    
                }
            }


            var analysticStack = new Stack<string>();
            foreach (var resource in starterPackList)
            {
                analysticStack.Push(resource);
                var currentNode = resource;
                do
                {
                    var nextProcesses = _processesController.ProcessesThatNeedsResource(currentNode);
                    foreach (var processName in nextProcesses)
                    {
                        var isRepeatProcess = inDeadLock.Contains(processName);
                        if (isRepeatProcess)
                            continue;

                        inDeadLock.Add(processName);
                        var process = processes[processName];
                        foreach (var availableResource in process.AvailableRes)
                        {
                            var resourceIdInStr = availableResource.ToString();
                            var isRepeatResource = inDeadLock.Contains(resourceIdInStr);
                            if (!isRepeatResource)
                            {
                                analysticStack.Push(resourceIdInStr);
                                inDeadLock.Add(resourceIdInStr);
                            }
                        }
                    }
                    currentNode = analysticStack.Pop();

                } while (analysticStack.Any());

            }


            return inDeadLock;
        }


        private Graph GetGraph(string graphName)
        {
            var graph = new Graph(graphName);

            foreach (var res in _resourcesAllocator.Resources)
            {
                var isFree = _resourcesAllocator.IsFreeResource(res.Key);
                if (!isFree)
                {
                    graph.AddEdge(res.Key.ToString(), res.Value);
                }
                else
                {
                    graph.AddNode(res.Key.ToString());
                }
            }

            foreach (var process in _processesController.Processes)
            {
                var node = graph.AddNode(process.Key);
                node.Attr.Shape = Shape.Box;
                foreach (var necessaryRes in process.Value.NecessaryRes)
                {
                    graph.AddEdge(process.Key, necessaryRes.ToString());
                }
            }

            return graph;
        }

        #endregion

    }
}

