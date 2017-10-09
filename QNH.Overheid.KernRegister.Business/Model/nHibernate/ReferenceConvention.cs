using FluentNHibernate.Conventions;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class ReferenceConvention : IReferenceConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance)
        {
            instance.Column("FK_" + instance.Name.ToUpperInvariant() + "_" + instance.Property.Name.ToUpperInvariant());
            instance.Index("IX_" + instance.Name.ToUpperInvariant() + "_" + instance.Property.Name.ToUpperInvariant());
        }
    }
}