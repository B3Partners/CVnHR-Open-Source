using System;
using NHibernate;

namespace QNH.Overheid.KernRegister.Business.Model
{
    public static class TransactionWrapper
    {
        public static void RunInTransaction(this ISession session, Action action)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    action();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    session.Dispose();
                    throw;
                }
            }
        }
    }
}