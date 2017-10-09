using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using QNH.Overheid.KernRegister.Business.Model.RSGB2_2;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class VSTGMapping : IAutoMappingOverride<VESTG>
    {
        public void Override(AutoMapping<VESTG> mapping)
        {
            mapping.Id(x => x.SC_IDENTIF);
        }
    }
}