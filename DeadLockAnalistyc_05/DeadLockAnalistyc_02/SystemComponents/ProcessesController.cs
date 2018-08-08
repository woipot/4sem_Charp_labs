using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_05
{
    internal sealed class ProcessesController : ICloneable
    {
        private readonly SortedDictionary<string, Process> _processesInSystem;

        public event EventHandler<short> FreeResourceEvent; 

        #region Properties

        public SortedDictionary<string, Process> Processes => _processesInSystem;

        #endregion



        #region Constructors

        public ProcessesController()
        {
            _processesInSystem = new SortedDictionary<string, Process>();
        }

        private ProcessesController(IDictionary<string, Process> processes)
        {
            _processesInSystem = new SortedDictionary<string, Process>();
            foreach (var procPair in processes)
            {
                _processesInSystem[procPair.Key] = (Process)procPair.Value.Clone();
            }

        }

        #endregion



        #region PublicMethods

        public void AddProcess(Process process)
        {
            _processesInSystem[process.Name] = process;
        }

        public void AddAvailableRes(string procName, short recourceId)
        {
            var process = _processesInSystem[procName];

            process.AvailableRes.Add(recourceId);

            var isProcessRequest = process.NecessaryRes.Contains(recourceId);
            if (isProcessRequest)
            {
                process.NecessaryRes.Remove(recourceId);
            }
        }

        public void AddRequestRes(string procName, short recourceId)
        {
            _processesInSystem[procName].NecessaryRes.Add(recourceId);
        }


        public void FreeResource(string procName, short resourceId)
        {
            var process = Processes[procName];

            var isAvailableRes = process.AvailableRes.Contains(resourceId);
            var isNesRes = process.NecessaryRes.Contains(resourceId);

            if (isAvailableRes)
            {
                process.AvailableRes.Remove(resourceId);
                FreeResourceEvent?.Invoke(this, resourceId);
            }
            else if (isNesRes)
            {
                process.NecessaryRes.Remove(resourceId);
            }

            var isDosntNeedResources = !process.AvailableRes.Any() &&
                                       !process.NecessaryRes.Any();
            if (isDosntNeedResources)
                _processesInSystem.Remove(procName);
        }

        public void CompleteRequest(object sender, short freeResId)
        {

            foreach (var process in Processes)
            {
                var isProcNeedsRes = process.Value.NecessaryRes.Contains(freeResId);
                if (isProcNeedsRes)
                {
                    process.Value.AvailableRes.Add(freeResId);
                    process.Value.NecessaryRes.Remove(freeResId);

                    var resAlloc = (ResourcesAllocator) sender;
                    resAlloc.MakeLink(freeResId, process.Key);
                    break;
                }

            }
        }


        public IEnumerable<string> ProcessesThatNeedsResource(short resourceId)
        {
            var result = new List<string>();
            foreach (var process in Processes)
            {
                var isNeed = process.Value.NecessaryRes.Contains(resourceId);
                if(isNeed)
                    result.Add(process.Key);
            }

            return result;
        }

        public IEnumerable<string> ProcessesThatNeedsResource(string resourceId)
        {
            short.TryParse(resourceId, out var result);
            return ProcessesThatNeedsResource(result);
        }


        public bool IsReallyProcess(string name)
        {
            var isNameConstains = _processesInSystem.Keys.Contains(name);
            return isNameConstains;
        }


        #endregion



        #region IClonable

        public object Clone()
        {
            return new ProcessesController(_processesInSystem);
        } 
        #endregion

    }
}
