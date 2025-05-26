using System.Text.Json.Serialization;

namespace MODAFurniture.Core.Entities;

public class Product
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("shortDescription")]
    public string ShortDescription { get; set; } = string.Empty;
    
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
    
    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;
    
    [JsonPropertyName("longDescription")]
    public string LongDescription { get; set; } = string.Empty;
}

public class ProductsContainer
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new List<Product>();
}
