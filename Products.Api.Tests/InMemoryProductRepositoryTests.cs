using Products.Api.Models;
using Products.Api.Repositories;

namespace Products.Api.Tests;

public class InMemoryProductRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_without_filter_returns_all_ordered_by_name()
    {
        var repo = new InMemoryProductRepository();
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "B",
            Color = "Blue",
            Price = 1
        });
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "A",
            Color = "Red",
            Price = 2
        });

        var all = await repo.GetAllAsync(null);

        Assert.Equal(2, all.Count);
        Assert.Equal(["A", "B"], all.Select(p => p.Name).ToArray());
    }

    [Fact]
    public async Task GetAllAsync_colour_filter_is_case_insensitive()
    {
        var repo = new InMemoryProductRepository();
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "One",
            Color = "Red",
            Price = 1
        });
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "Two",
            Color = "Blue",
            Price = 2
        });

        var reds = await repo.GetAllAsync("RED");

        var list = Assert.Single(reds);
        Assert.Equal("Red", list.Color);
    }

    [Fact]
    public async Task GetAllAsync_whitespace_colour_treated_as_no_filter()
    {
        var repo = new InMemoryProductRepository();
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "One",
            Color = "Green",
            Price = 1
        });

        var all = await repo.GetAllAsync("   ");

        Assert.Single(all);
    }
}
