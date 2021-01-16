using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Identity;

namespace DBCache
{
    public class CustomIdentityStep : IAutomappingStep
    {
        private readonly IAutomappingConfiguration _cfg;

        public CustomIdentityStep(IAutomappingConfiguration cfg)
        {
            _cfg = cfg;
        }

        public bool ShouldMap(Member member)
        {
            return _cfg.IsId(member);
        }

        public void Map(ClassMappingBase classMap, Member member)
        {
            if (!(classMap is ClassMapping)) return;

            IdMapping idMapping = new IdMapping { ContainingEntityType = classMap.Type };
            ColumnMapping columnMapping = new ColumnMapping();
            columnMapping.Set(x => x.Name, Layer.Defaults, member.Name);
            idMapping.AddColumn(Layer.Defaults, columnMapping);
            idMapping.Set(x => x.Name, Layer.Defaults, member.Name);
            idMapping.Set(x => x.Type, Layer.Defaults, new TypeReference(member.PropertyType));
            idMapping.Member = member;
            idMapping.Set(x => x.Generator, Layer.Defaults, GetDefaultGenerator(member));

            SetDefaultAccess(member, idMapping);

            ((ClassMapping)classMap).Set(x => x.Id, Layer.Defaults, idMapping);        
        }

        private void SetDefaultAccess(Member member, IdMapping mapping)
        {
            Access resolvedAccess = MemberAccessResolver.Resolve(member);

            if (resolvedAccess != Access.Property && resolvedAccess != Access.Unset)
            {
                // if it's a property or unset then we'll just let NH deal with it, otherwise
                // set the access to be whatever we determined it might be
                mapping.Set(x => x.Access, Layer.Defaults, resolvedAccess.ToString());
            }

            if (member.IsProperty && !member.CanWrite)
                mapping.Set(x => x.Access, Layer.Defaults, _cfg.GetAccessStrategyForReadOnlyProperty(member).ToString());
        }

        private GeneratorMapping GetDefaultGenerator(Member property)
        {
            GeneratorMapping generatorMapping = new GeneratorMapping();
            generatorMapping.Set(x => x.Class, Layer.Defaults, "assigned");

            return generatorMapping;
        }
    }
}
