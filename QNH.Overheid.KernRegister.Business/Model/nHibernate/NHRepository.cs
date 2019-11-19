using NHibernate;
using System.Linq;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class NHRepository<T> : IRepository<T> where T : class
    {
        private readonly ISession _session;

        public NHRepository(ISession session)
        {
            _session = session;
        }
        
        public virtual void AddOrUpdate(T entity)
        {
            _session.RunInTransaction(() =>
            {
                if (_session.Contains(entity))
                    _session.Evict(entity);
                _session.Merge(entity);
                _session.Flush();
            });
        }

        public virtual void AddOrUpdateOther<T2>(T2 entity) where T2 : class
        {
            _session.RunInTransaction(() =>
            {
                if(_session.Contains(entity))
                    _session.Evict(entity);
                _session.Merge(entity);
                _session.Flush();
            });
        }

        public virtual void Remove(T entity)
        {
            _session.RunInTransaction(() =>
            {
                if (_session.Contains(entity))
                    _session.Evict(entity);
                _session.Delete(entity);
                _session.Flush();
            });
        }

        public virtual void RemoveOther<T2>(T2 entity)
        {
            _session.RunInTransaction(() =>
            {
                if (_session.Contains(entity))
                    _session.Evict(entity);
                _session.Delete(entity);
                _session.Flush();
            });
        }

        public IQueryable<T> Query() => _session.Query<T>();

        public IQueryable<T2> QueryOther<T2>() where T2 : class => _session.Query<T2>();

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
