using System;

namespace ORM_DEV
{
    /// <summary>
    /// EntityBase defines fields which are required by every Entity
    /// </summary>
    public abstract class EntityBase : IEquatable<EntityBase>
    {
        /// <summary>
        /// The Id property is required for each and every entity
        /// </summary>
        public long Id { get; }

        public bool Equals(EntityBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EntityBase) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}