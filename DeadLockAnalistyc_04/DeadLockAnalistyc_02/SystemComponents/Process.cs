using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_04
{
    internal sealed class Process : ICloneable
    {
        private string _name;
        private List<short> _availableResources;
        private List<short> _necessaryResources;

        
        
        #region Properties

        public string Name => _name;

        public List<short> AvailableRes => _availableResources;

        public List<short> NecessaryRes => _necessaryResources;

        #endregion



        #region Constructors

        public Process(string name)
        {
            _name = name;
            _availableResources = new List<short>();
            _necessaryResources = new List<short>();
        }

        public Process(string name, IEnumerable<short> availableRes, IEnumerable<short> necessaryRes)
        {
            _name = name;
            _availableResources = new List<short>(availableRes);
            _necessaryResources = new List<short>(necessaryRes);
        }
        #endregion



        #region Public Methods

        /*public void Action(short id, ActionEnum action)
        {
            switch (action)
            {
                case ActionEnum.Release:
                    var isAlreadyAvailable = _availableResources.Contains(id);
                    if (isAlreadyAvailable)
                    {
                        _availableResources.Remove(id);
                    }
                    else
                    {
                        throw new Exception($"#Error: Procces {_name} don't hase resurce with id {id}");
                    }
                    break;

                case ActionEnum.Request:
                    var isAlreadyRequest = _availableResources.Contains(id);
                    if (isAlreadyRequest)
                    {
                        _necessaryResources.Add(id);
                    }
                    else
                    {
                        throw new Exception($"#Error: Procces {_name} don't hase resurce with id {id}");
                    }
                    break;
            }
        }*/

        #endregion



        #region ICLonable

        public object Clone()
        {
            return new Process(_name, _availableResources, _necessaryResources);
        }

        #endregion

    }
}
