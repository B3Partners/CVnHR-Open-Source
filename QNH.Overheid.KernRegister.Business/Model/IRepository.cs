using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QNH.Overheid.KernRegister.Business.Model
{
    public interface IRepository<T> : IDisposable
    {
        void AddOrUpdate(T entity);
        void Remove(T entity);
        IQueryable<T> Query();
    }
}
