using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QNH.Overheid.KernRegister.Beheer
{
    public class AccessDeniedModel
    {
        public IEnumerable<string> Administrators { get; set; }
        public IEnumerable<ApplicationActions> DeniedPermission { get; set; }
        public bool Any { get; internal set; }
    }
}