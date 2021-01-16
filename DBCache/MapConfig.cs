using System;
using System.Collections.Generic;
using System.Reflection;
using DBCache.Attributes;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Conventions;

namespace DBCache
{
    public class MapConfig : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.IsSubclassOf(typeof(Entity));
        }

        public override bool ShouldMap(Member member)
        {
            if (member.MemberInfo.GetCustomAttribute<NotMappedAttribute>() != null)
            {
                return false;
            }
            
            return base.ShouldMap(member);
        }

        public override bool IsId(Member member)
        {
            if (member.MemberInfo.GetCustomAttribute<IdAttribute>() != null)
            {
                return true;
            }
            
            return base.IsId(member);
        }

        public override IEnumerable<IAutomappingStep> GetMappingSteps(AutoMapper mapper, IConventionFinder conventionFinder)
        {
            return new IAutomappingStep[]
            {
                new CustomIdentityStep(this),
                new VersionStep(this),
                new ComponentStep(this),
                new PropertyStep(conventionFinder, this),
                new HasManyToManyStep(this),
                new ReferenceStep(this),
                new HasManyStep(this)
            };
            //return base.GetMappingSteps(mapper, conventionFinder);
        }
    }
}