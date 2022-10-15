using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Core
{
    public sealed class LoginRejectedException : Exception
    {
        public LoginRejectedException(string reason) : base(reason)
        {

        }
    }
}
