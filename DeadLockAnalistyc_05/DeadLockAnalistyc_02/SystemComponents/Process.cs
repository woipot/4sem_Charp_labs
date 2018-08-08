using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_05
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



        #region ICLonable

        public object Clone()
        {
            return new Process(_name, _availableResources, _necessaryResources);
        }

        #endregion

    }
}
