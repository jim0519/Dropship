using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    /// <summary>
    /// A container for entities that have been inserted.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityInserted<T> where T : BaseEntity
    {
        public EntityInserted(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }

    /// <summary>
    /// A container for entities that are updated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityUpdated<T> where T : BaseEntity
    {
        public EntityUpdated(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }

    /// <summary>
    /// A container for passing entities that have been deleted. This is not used for entities that are deleted logicaly via a bit column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityDeleted<T> where T : BaseEntity
    {
        public EntityDeleted(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }

   
    public class ListingPriceUpdated<T> where T : BaseEntity
    {
        public ListingPriceUpdated(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }

    public class ListingPostageRuleUpdated<T> where T : BaseEntity
    {
        public ListingPostageRuleUpdated(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}
