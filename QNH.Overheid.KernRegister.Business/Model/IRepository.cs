using System;
using System.Linq;

namespace QNH.Overheid.KernRegister.Business.Model
{
    public interface IRepository<T> : IDisposable
    {
        void AddOrUpdate(T entity);
        void Remove(T entity);
        IQueryable<T> Query();
    }
}
