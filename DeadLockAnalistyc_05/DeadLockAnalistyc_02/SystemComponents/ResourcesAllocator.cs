using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_05
{
    internal sealed class ResourcesAllocator : ICloneable
    {
        private SortedDictionary<short, string> _resourcesInSystem;
        public EventHandler<short> ResourceReliaseEvent; 


        #region Constructors
        public ResourcesAllocator()
        {
            _resourcesInSystem = new SortedDictionary<short, string>();
        }

        private ResourcesAllocator(IDictionary<short, string> ress)
        {
            _resourcesInSystem = new SortedDictionary<short, string>(ress);
        }
        #endregion
        


        #region Properties

        public SortedDictionary<short, string> Resources => _resourcesInSystem;

        #endregion



        #region Public Methods
        public void AddResource(short resourceId)
        {
            _resourcesInSystem.Add(resourceId, null);
        }


        public void MakeLink(short resourceId, string processName)
        {
            _resourcesInSystem[resourceId] = processName;
        }
        
        public void DeleteLink(short resourceId)
        {
            Resources[resourceId] = null;

            ResourceReliaseEvent(this, resourceId);
        }


        public bool IsFreeResource(short resourceId)
        {
            var isFree = _resourcesInSystem[resourceId] == null || _resourcesInSystem[resourceId] == "";
            return isFree;
        }

        public bool IsReallyResource(short resourceId)
        {
            var isConstaints = _resourcesInSystem.ContainsKey(resourceId);
            return isConstaints;
        }

        public bool IsReallyAndFreeResource(short resourceId)
        {
            var isReallyAndFree = IsReallyResource(resourceId) && IsFreeResource(resourceId);
            return isReallyAndFree;
        }
        #endregion

        
        #region IClonable

        public object Clone()
        {
            return new ResourcesAllocator(_resourcesInSystem);
        }

        #endregion

    }
}
