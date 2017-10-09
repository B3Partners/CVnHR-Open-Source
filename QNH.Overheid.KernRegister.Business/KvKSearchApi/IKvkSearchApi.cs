using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QNH.Overheid.KernRegister.Business.KvKSearchApi.Entities;

namespace QNH.Overheid.KernRegister.Business.KvKSearchApi
{
    public interface IKvkSearchApi
    {
        KvkSearchApiResult Search(KvkSearchApiParameters parameters);
    }

    public class KvkSearchApi : IKvkSearchApi
    {
        public KvkSearchApiResult Search(KvkSearchApiParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
