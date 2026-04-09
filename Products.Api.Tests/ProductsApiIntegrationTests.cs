using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Products.Api.Models;

namespace Products.Api.Tests;

public class ProductsApiIntegrationTests : IClassFixture<ProductsApiFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProductsApiIntegrationTests(ProductsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_products_without_token_returns_unauthorized()
    {
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Products_round_trip_with_jwt_and_colour_filter()
    {
        var token = await RequestTokenAsync();
        Assert.False(string.IsNullOrEmpty(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDesk = await _client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Name = "Desk",
            Color = "Oak",
            Price = 199.99m
        });
        Assert.Equal(HttpStatusCode.Created, createDesk.StatusCode);

        var createChair = await _client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Name = "Chair",
            Color = "Red",
            Price = 89m
        });
        Assert.Equal(HttpStatusCode.Created, createChair.StatusCode);

        var oakOnly = await _client.GetFromJsonAsync<List<Product>>("/api/products?colour=oak", JsonOptions);
        Assert.NotNull(oakOnly);
        Assert.Single(oakOnly);
        Assert.Equal("Oak", oakOnly[0].Color);
    }

    private async Task<string?> RequestTokenAsync()
    {
        var response = await _client.PostAsync(
            "/api/auth/token",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.GetProperty("accessToken").GetString();
    }
}
