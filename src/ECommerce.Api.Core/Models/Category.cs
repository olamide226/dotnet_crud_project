using System;
using System.ComponentModel.DataAnnotations;
using DotnetCrud.Core.Models;

namespace ECommerce.Api.Core.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // TODO: Add your entity-specific properties here
        // Example:
        // public decimal Price { get; set; }
        // public int Quantity { get; set; }
        // public string Category { get; set; }
    }
}
