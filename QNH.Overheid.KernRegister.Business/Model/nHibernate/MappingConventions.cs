using QNH.Overheid.KernRegister.Business.Model.Entities;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using QNH.Overheid.KernRegister.Business.Model.Entities.Brmo;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class CustomMappingConfiguration : DefaultAutomappingConfiguration
    {
        private static readonly List<string> _nameSpacesToMap = new List<string>()
        {
            "QNH.Overheid.KernRegister.Business.Model.Entities",
            //"QNH.Overheid.KernRegister.Business.Model.Entities.Brmo"
        };

        public override bool IsConcreteBaseType(Type type)
        {
            // Allow inheritance of classes
            return _nameSpacesToMap.Contains(type.Namespace);
        }

        public override bool ShouldMap(Type type)
        {
            return _nameSpacesToMap.Contains(type.Namespace)
                && base.ShouldMap(type);
        }

        public override bool ShouldMap(Member member)
        {
            return member.IsProperty
                && (member.IsProtected || member.IsPublic)
                && member.CanWrite 
                && base.ShouldMap(member);
        }
    }

    public class TableNameConvention : IClassConvention
    {
        private Action<IClassInstance> _applyAction;

        public TableNameConvention(Action<IClassInstance> applyAction = null)
        {
            _applyAction = applyAction;
        }

        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.ToUpperInvariant());

            _applyAction?.Invoke(instance);
        }
    }

    public class ColumnNameConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            instance.Column(instance.Property.Name.ToUpperInvariant());
        }
    }

    public class IndexableConvention : AttributePropertyConvention<IndexableAttribute>
    {
        protected override void Apply(IndexableAttribute attribute, IPropertyInstance instance)
        {
            var name = string.IsNullOrWhiteSpace(attribute.Name)
                ? "IX_" + instance.EntityType.Name.ToUpperInvariant() + "_" + instance.Property.Name.ToUpperInvariant()
                : attribute.Name.StartsWith("IX_") ? attribute.Name : "IX_" + attribute.Name;
            instance.Index(name);
        }
    }

    public class SbiCodeMapping : IAutoMappingOverride<SbiCode>
    {
        public void Override(AutoMapping<SbiCode> mapping)
        {
            mapping.Id(x => x.Code);
        }
    }

    public class ReferenceConvention : IReferenceConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance)
        {
            instance.Column("FK_" + instance.Name.ToUpperInvariant() + "_" + instance.Property.Name.ToUpperInvariant());
            instance.Index("IX_" + instance.Name.ToUpperInvariant() + "_" + instance.Property.Name.ToUpperInvariant());
        }
    }

    public class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention, IHasOneConvention
    {
        public void Apply(IOneToOneInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cascade.AllDeleteOrphan();
        }

        // http://stackoverflow.com/questions/2058417/nhibernate-definitive-cascade-application-guide

        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.AllDeleteOrphan();
        }
    }
}
