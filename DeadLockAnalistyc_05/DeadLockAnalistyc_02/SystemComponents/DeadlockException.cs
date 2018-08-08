using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockAnalistyc_05
{
    public sealed class DeadlockException : Exception
    {
        public DeadlockException(string message) : base(message)
        {
        }
    
    }
}
