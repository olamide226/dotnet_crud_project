using System;
using System.ComponentModel.DataAnnotations;
using DotnetCrud.Core.Models;

namespace ECommerce.Api.Core.Models
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;
        
        public bool IsFeatured { get; set; }
        
        public string[] ImageUrls { get; set; } = [];
    }
}
