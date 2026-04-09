using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Models;
using Products.Api.Repositories;

namespace Products.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(
        [FromQuery] string? colour,
        CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(colour, cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Color))
        {
            return BadRequest("Color is required.");
        }

        if (request.Price < 0)
        {
            return BadRequest("Price cannot be negative.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Color = request.Color.Trim(),
            Price = request.Price
        };

        await _repository.AddAsync(product, cancellationToken);
        return Created($"/api/products/{product.Id}", product);
    }
}
