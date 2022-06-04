using McProtoNet.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Core
{
    public interface ISession
    {
        void Close();
        Task<bool> Login();
    }
}
