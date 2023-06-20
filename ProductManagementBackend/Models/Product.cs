using System.ComponentModel.DataAnnotations;

namespace ProductManagementBackend.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string? Image { get; set; }
}

public class ProductRequest
{
    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public IFormFile Image { get; set; }
}
