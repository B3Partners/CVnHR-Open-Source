using NHibernate;
using Oracle.ManagedDataAccess.Client;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class RsgbRepository<T> : NHRepository<T>, IRsgbRepository<T> where T : class
    {
        public RsgbRepository(ISession session) 
            : base(session)
        {
        }
    }
}