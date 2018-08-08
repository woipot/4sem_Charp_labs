using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_04
{
    internal sealed class ResourcesAllocator : ICloneable
    {
        private SortedDictionary<short, string> _resourcesInSystem;



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
            if (CkeckRes(resourceId))
            {
                throw new Exception($"#Error: resource [{resourceId}] already constaint in system");
            }

            _resourcesInSystem.Add(resourceId, null);

        }

        
        public void MakeLink(short resourceId, string processName)
        {
            if (!CkeckRes(resourceId))
            {
                throw new Exception($"#Error: It is Imposible to give the process [{processName}] resource [{resourceId}]. Resource is absent in system");
            }

            if (!CheckResToFree(resourceId))
            {
                throw new Exception($"#Error: It is Imposible to give the processName [{processName}] resource [{resourceId}]. Another process is already using it");
            }

            _resourcesInSystem[resourceId] = processName;
            
        }
        
        public bool CheckResToFree(short resourceId)
        {
            if (!CkeckRes(resourceId))
            {
                return false;
            }

            var isFree = _resourcesInSystem[resourceId] == null || _resourcesInSystem[resourceId] == "";
            return isFree;
        }

        public bool CkeckRes(short resourceId)
        {
            var isConstaints = _resourcesInSystem.ContainsKey(resourceId);
            return isConstaints;
        }



        public void DeleteLink(short resourceId)
        {
            if(!CkeckRes(resourceId))
                throw new Exception($"#Error: Imposible delete link. Resource with id [{resourceId}] is absent in the system");

            if(CheckResToFree(resourceId))
                throw new Exception($"#Error: Imposible delete link. Resource with id [{resourceId}] is already free");

            Resources[resourceId] = null;
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
