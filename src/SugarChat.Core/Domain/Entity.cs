using System;

namespace SugarChat.Core.Domain
{
    public abstract class Entity<T> : IEntity<T>
    {
        public virtual T Id { get; protected set; }
    }

    public abstract class Entity : Entity<Guid>
    {
    }
}