using McProtoNet.API.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.API
{
    public interface ISession
    {
        void Close();
        Task<bool> Login();
    }
}
