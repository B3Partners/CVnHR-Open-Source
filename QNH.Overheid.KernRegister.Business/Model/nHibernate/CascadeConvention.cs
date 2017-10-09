using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            //instance.Cascade.All();
            instance.Cascade.AllDeleteOrphan();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            //instance.Cascade.All();
            instance.Cascade.AllDeleteOrphan();
        }
    }
}