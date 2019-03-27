using NHibernate;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using System.Linq;

namespace QNH.Overheid.KernRegister.Business.Utility
{
    public static class NHibernateUtils
    {
        public static string GetGeneratedSql(this IQueryable queryable, ISession session)
        {
            var sessionImp = (ISessionImplementor)session;
            var nhLinqExpression = new NhLinqExpression(queryable.Expression, sessionImp.Factory);
            var translatorFactory = new ASTQueryTranslatorFactory();
            var translators = translatorFactory.CreateQueryTranslators(nhLinqExpression, nhLinqExpression.Key, false, sessionImp.EnabledFilters, sessionImp.Factory);

            return translators[0].SQLString;
        }
    }
}
