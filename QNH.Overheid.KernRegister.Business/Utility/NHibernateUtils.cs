using NHibernate;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QNH.Overheid.KernRegister.Business.Utility
{
    public static class NHibernateUtils
    {
        public static String GetGeneratedSql(this IQueryable queryable, ISession session)
        {
            var sessionImp = (ISessionImplementor)session;
            var nhLinqExpression = new NhLinqExpression(queryable.Expression, sessionImp.Factory);
            var translatorFactory = new ASTQueryTranslatorFactory();
            var translators = translatorFactory.CreateQueryTranslators(nhLinqExpression.Key, nhLinqExpression.Key, false, sessionImp.EnabledFilters, sessionImp.Factory);
                //translatorFactory.CreateQueryTranslators(nhLinqExpression.Key, nhLinqExpression, null, false, sessionImp.EnabledFilters, sessionImp.Factory);

            return translators[0].SQLString;
        }
    }
}
