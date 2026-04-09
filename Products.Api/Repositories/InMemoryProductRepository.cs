using System.Collections.Concurrent;
using Products.Api.Models;

namespace Products.Api.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _items = new();

    public Task<IReadOnlyList<Product>> GetAllAsync(string? colour, CancellationToken cancellationToken = default)
    {
        IEnumerable<Product> query = _items.Values.OrderBy(p => p.Name);

        if (!string.IsNullOrWhiteSpace(colour))
        {
            var needle = colour.Trim();
            query = query.Where(p =>
                string.Equals(p.Color, needle, StringComparison.OrdinalIgnoreCase));
        }

        IReadOnlyList<Product> list = query.ToList();
        return Task.FromResult(list);
    }

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _items[product.Id] = product;
        return Task.FromResult(product);
    }
}
