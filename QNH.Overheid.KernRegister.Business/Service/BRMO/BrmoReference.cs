using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Service.BRMO
{
    public class BrmoReference
    {
        public string BaseUrl { get; set; } = "http://localhost:8080/brmo-service";

        public string BrmoUrl => BaseUrl.Trim('/');
        public string UserName { get; set; } = "brmo";
        public string Password { get; set; } = "brmo";

        public string NhrServiceUrl => string.Join("/", BrmoUrl, "post", "nhr");
    }
}
