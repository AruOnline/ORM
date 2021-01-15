using System;

namespace ORM_DEV.Framework.Entities
{
    /// <summary>
    /// EntityBase defines fields which are required by every Entity
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// The Id property is required for each and every entity
        /// </summary>
        public long Id { get; protected set; }
    }
}