using System;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using ORM_TEST.Framework.Attributes;

namespace ORM_TEST.Framework
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
    }
}