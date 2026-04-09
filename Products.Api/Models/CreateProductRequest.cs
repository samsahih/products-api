namespace Products.Api.Models;

public class CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
