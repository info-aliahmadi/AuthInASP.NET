using System;

namespace AuthInASP.NET.Domian
{
    /// <summary>
    /// Represents the base class for entities
    /// </summary>
    public abstract class BaseEntity<PKType>
    {
        public PKType Id { get; set; }

    }
}