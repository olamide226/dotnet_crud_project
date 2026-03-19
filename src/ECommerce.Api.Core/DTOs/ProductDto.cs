using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Core.DTOs
{
    public class CreateProductDto
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
        
        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;
        
        public bool IsFeatured { get; set; }
    }

    public class UpdateProductDto
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
        public string Category { get; set; } = string.Empty;
        
        public bool IsFeatured { get; set; }
    }

    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public string[] ImageUrls { get; set; } = [];
        public bool InStock => StockQuantity > 0;
    }
}
