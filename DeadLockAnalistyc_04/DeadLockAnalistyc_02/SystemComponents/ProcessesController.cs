using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_04
{
    internal sealed class ProcessesController : ICloneable
    {
        private SortedDictionary<string, Process> _processesInSystem;



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
            var inSystem = ProcessInSystem(process.Name);
            if (inSystem)
            {
                throw new Exception($"#Error: Process with a same name [{process.Name}] is already used in system");
            }

            _processesInSystem[process.Name] = process;
        }



        public Process AddAvailableRes(string procName, short recourceId)
        {
            var inSystem = ProcessInSystem(procName);
            if(!inSystem)
                throw new Exception($"#Error: Impossible to get process [{procName}] resource. Process doesn't constains in system");

            _processesInSystem[procName].AvailableRes.Add(recourceId);
            return _processesInSystem[procName];
        }



        public Process AddRequestRes(string procName, short recourceId)
        {
            var inSystem = ProcessInSystem(procName);
            if (!inSystem)
                throw new Exception($"#Error: Process [{procName}] can't request a resource. Process doesn't constains in system");

            _processesInSystem[procName].NecessaryRes.Add(recourceId);
            return _processesInSystem[procName];
        }



        public bool ProcessInSystem(string name)
        {
            var isNameConstains = _processesInSystem.Keys.Contains(name);
            return isNameConstains;
        }



        public void FreeRes(string procName, short resourceId)
        {
            var inSystem = ProcessInSystem(procName);
            if (!inSystem)
                throw new Exception($"#Error: Process [{procName}] can't free a resource. Process doesn't constains in system");

            var isConstainRes = Processes[procName].AvailableRes.Contains(resourceId);
            if(!isConstainRes)
                throw new Exception($"#Error: Process [{procName}] can't free a resource. Process hasn't resource with id [{resourceId}] ");

            Processes[procName].AvailableRes.Remove(resourceId);
        }



        public void CanselRequest(string procName, short resourceId)
        {
            var inSystem = ProcessInSystem(procName)
                ;
            if (!inSystem)
                throw new Exception($"#Error: Process [{procName}] can't cansel resource's request. Process doesn't constains in system");

            var isConstainRes = Processes[procName].NecessaryRes.Contains(resourceId);
            if (!isConstainRes)
                throw new Exception($"#Error: Process [{procName}] can't cansel resource's request. Process dosn't need the resource with id [{resourceId}] ");

            Processes[procName].NecessaryRes.Remove(resourceId);
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
