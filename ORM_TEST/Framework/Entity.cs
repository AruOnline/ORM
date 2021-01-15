using FluentNHibernate.Mapping;

namespace ORM_TEST.Framework
{
    public abstract class Entity
    {
        public virtual long Id { get; protected set; }

        public T Get<T>() where T : Entity
        {
            return new { } as T;
        }
    }
}