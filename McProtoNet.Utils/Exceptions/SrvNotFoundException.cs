using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Utils
{
    public sealed class SrvNotFoundException : Exception
    {
        public SrvNotFoundException():base("Srv record not found") { }
    }
}
