using System;

namespace DotnetCrud.Core.Models
{
    /// <summary>
    /// Base entity class that all domain entities should inherit from
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Date and time when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Indicates if the entity has been soft deleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Optional: User who created the entity
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Optional: User who last updated the entity
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Constructor initializes CreatedAt to current UTC time
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}