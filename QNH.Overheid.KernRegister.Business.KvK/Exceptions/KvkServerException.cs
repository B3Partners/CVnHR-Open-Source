using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.KvK.Exceptions
{
    public class KvkServerException : Exception
    {
        public KvkServerException(string msg) : base(msg)
        { }
    }
}
