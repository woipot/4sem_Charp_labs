using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeadLockAnalistyc_04;
using Microsoft.Glee.Drawing;

namespace DeadLockAnalistyc_04
{
    public sealed class MySystem
    {
        private ProcessesController _processesController;
        private ResourcesAllocator  _resourcesAllocator;



        #region Constructors

        public MySystem()
        {
            _resourcesAllocator  = new ResourcesAllocator();
            _processesController = new ProcessesController();
        }

        #endregion



        #region Public methods

        public void InitResource(short resourceId)
        {
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



        public void InitProcess(string procName, IEnumerable<short> availableRes, IEnumerable<short> requestRes)
        {

            var dosntNeedRes = !availableRes.Any() && !requestRes.Any();
            if (dosntNeedRes)
            {
                var newProcess = new Process(procName, availableRes, requestRes);
                _processesController.AddProcess(newProcess);
            }
            else
            {
                foreach (var resourceId in availableRes)
                {
                    var inSystem = _resourcesAllocator.CkeckRes(resourceId);
                    if(!inSystem)
                        throw new Exception($"#Error: Impossible to init process [{procName}], because system doesn't constains resource with id [{resourceId}]");

                    var isfree = _resourcesAllocator.CheckResToFree(resourceId);
                    if(!isfree)
                        throw new Exception($"#Error: Impossible to init process [{procName}], because another process already uses resource with id [{resourceId}]");
                }

                foreach (var resourceId in requestRes)
                {
                    var inSystem = _resourcesAllocator.CkeckRes(resourceId);
                    if (!inSystem)
                        throw new Exception($"#Error: Impossible to init process [{procName}], because system doesn't constains resource with id [{resourceId}]");
                }

                if (IsRepeat(availableRes) || IsRepeat(requestRes))
                    throw new Exception($"#Error: One Process [{procName}] can't request (pick up) a single resource twise");

                var newProcess = new Process(procName, availableRes, requestRes);
                var resAllocScan = (ResourcesAllocator)_resourcesAllocator.Clone();
                var procCtrlScan = (ProcessesController)_processesController.Clone();

                foreach (var resourceId in availableRes)
                {
                    resAllocScan.MakeLink(resourceId, newProcess.Name);
                }
                procCtrlScan.AddProcess(newProcess);

                try
                {
                    SafeCheck(resAllocScan, procCtrlScan);
                }
                catch (Exception e)
                {

                    var additionMessage = $"#Error: Impossible to init process [{procName}], because this action goes to deadlock:\n";
                    throw new Exception(additionMessage + e.Message);
                }

                _resourcesAllocator = resAllocScan;
                _processesController = procCtrlScan;
                
            }
        }

        public void InitProcess(string procName, IEnumerable<string> availableResStrArr, IEnumerable<string> requestResStrArr)
        {
            var availableRes = new List<short>();
            var reqestRes = new List<short>();


            foreach (var resStr in availableResStrArr)
            {
                var isShort = short.TryParse(resStr, out var res);

                if (!isShort)
                {
                    throw new Exception($"#Error: Wrong argument in init process [{procName}] in available res [{resStr}]");
                }
                availableRes.Add(res);
            }

            foreach (var resStr in requestResStrArr)
            {
                var isShort = short.TryParse(resStr, out var res);
                if (!isShort)
                {
                    throw new Exception($"#Error: Wrong argument in init process [{procName}] in request res [{resStr}]");
                }

            reqestRes.Add(res);
            }

            InitProcess(procName, availableRes, reqestRes);
        }



        public void ProcGetRes(string procName, short resourceId)
        {
            var resAllocScan = (ResourcesAllocator)_resourcesAllocator.Clone();
            var procCtrlScan = (ProcessesController)_processesController.Clone();

            var proc = procCtrlScan.AddAvailableRes(procName, resourceId);
            resAllocScan.MakeLink(resourceId, proc.Name);

            try
            {
                SafeCheck(resAllocScan, procCtrlScan);
            }
            catch (Exception e)
            {

                var additionMessage = $"#Error: Impossible to init process [{procName}], because this action goes to deadlock:\n";
                throw new Exception(additionMessage + e.Message);
            }

            _resourcesAllocator = resAllocScan;
            _processesController = procCtrlScan;

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



        public void RequestGetRes(string procName, short resourceId)
        {
            var procCtrlScan = (ProcessesController)_processesController.Clone();
            procCtrlScan.AddRequestRes(procName, resourceId);

            try
            {
                SafeCheck(_resourcesAllocator, procCtrlScan);

            }
            catch (Exception e)
            {

                var additionMessage = $"#Error: Impossible to init process [{procName}], because this action goes to deadlock:\n";
                throw new Exception(additionMessage + e.Message);
            }

            _processesController = procCtrlScan;

        }

        public void RequestGetRes(string procName, string resourceId)
        {
            var isShort = short.TryParse(resourceId, out var res);

            if (!isShort)
            {
                throw new Exception($"#Error: Process [{procName}] can't request a resource. Wrong argument in resource id [{resourceId}]");
            }


            RequestGetRes(procName, res);
        }



        public void ProcFreeRes(string procName, short resourceId)
        {

            _processesController.FreeRes(procName, resourceId);
            _resourcesAllocator.DeleteLink(resourceId);

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



        public void CanselRequest(string procName, short resourceId)
        {

            _processesController.CanselRequest(procName, resourceId);

        }

        public void CanselRequest(string procName, string resourceId)
        {
            var isShort = short.TryParse(resourceId, out var res);

            if (!isShort)
            {
                throw new Exception($"#Error: Process [{procName}] can't cancel resource's request. Wrong argument");
            }

            CanselRequest(procName, res);
        }


        public Graph GetGraph(string graphName)
        {
            var graph = new Graph(graphName);
            
            foreach (var res in _resourcesAllocator.Resources)
            {
                var isFree = _resourcesAllocator.CheckResToFree(res.Key);
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


        private static void SafeCheck(ResourcesAllocator resAlloc, ProcessesController procCtrl)
        {

            var processes = procCtrl.Processes;
            var resources = resAlloc.Resources;

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
                        
                        var isProcess = procCtrl.ProcessInSystem(currentNode);
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
                            var message = GetMeassageFromDeadlock(analistycStack, procCtrl);
                            throw new Exception(message);
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


        private static string GetMeassageFromDeadlock(IEnumerable<string> analystic, ProcessesController procCtrl)
        {
            var sb = new StringBuilder();
            var resInDeadlock = new List<string>();
            var procInDeadlock = new List<string>();

            var analysticList = analystic.ToList();
            var index = analysticList.Count - 1;
            analysticList.RemoveAt(index);

            var processes = procCtrl.Processes;

            foreach (var id in analysticList)
            {
                var isProcess = processes.ContainsKey(id);
                if (isProcess)
                {
                    procInDeadlock.Add(id);
                    foreach (var res in processes[id].AvailableRes)
                    {
                        resInDeadlock.Add(res.ToString());
                    }
                }


            }

            sb.Append("{\nProcess in deadlock:\n");

            var counter = 1;
            foreach (var procName in procInDeadlock)
            {
                sb.Append("\t" + counter + ") " + procName + "\n");
                ++counter;
            }

            sb.Append("Resources in deadLock:\n");
            counter = 1;
            foreach (var resName in resInDeadlock)
            {
                sb.Append("\t" + counter + ") " + resName + "\n");
                ++counter;
            }

            sb.Append("}");

            return sb.ToString();
        }
        #endregion

    }
}

