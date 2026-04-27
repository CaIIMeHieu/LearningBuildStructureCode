using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Test.Integration.Infrastructure;

namespace Test.Integration.Products;

public class GetProductsTests : IClassFixture<LearnLoopWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetProductsTests(LearnLoopWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturn200()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/GetProductsGood");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
