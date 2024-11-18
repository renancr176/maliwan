using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.OrderCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Domain.MaliwanContext.Enums;
using Maliwan.Service.Api;
using Maliwan.Service.Api.Models.Responses;
using Maliwan.Test.Extensions;
using Maliwan.Test.IntegrationTests.Config;
using Microsoft.EntityFrameworkCore;

namespace Maliwan.Test.IntegrationTests;

[Collection(nameof(IntegrationTestsFixtureCollection))]
public class OrderControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public OrderControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get order by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingOrder_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Orders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewOrderAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<BaseResponse<OrderModel?>>($"/Order/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Theory(DisplayName = "Search orders should at least get a success response code 200/OK")]
    [InlineData("ByCustomer")]
    [InlineData("BySellDate")]
    [InlineData("ByCustomerAndSellDate")]
    public async Task Search_UserOrders_ShouldQuerySuccessfully(string testType)
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Orders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue && !e.OrderItems.Any())
                     ?? await _testsFixture.GetInsertedNewOrderAsync();

        OrderSearchRequest request = new OrderSearchRequest();

        switch (testType)
        {
            case "ByCustomer":
                request = new OrderSearchRequest(idCustomer: entity.IdCustomer);
                break;
            case "BySellDate":
                request = new OrderSearchRequest(sellDate: entity.SellDate);
                break;
            case "ByCustomerAndSellDate":
                request = new OrderSearchRequest(idCustomer: entity.IdCustomer, sellDate: entity.SellDate);
                break;
        }

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<OrderSearchRequest, BaseResponse<PagedResponse<OrderModel>?>>("/Order/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid order should be created successfully.")]
    public async Task Create_GivenValidOrder_ShouldCreateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.GetNewOrderAsync();
        var items = new List<ProductItem>();
        foreach (var orderItem in entity.OrderItems)
        {
            var stock = await _testsFixture.MaliwanDbContext.Stocks
                .FirstAsync(e => e.Id == orderItem.IdStock);
            items.Add(new ProductItem(stock.IdProduct, orderItem.Quantity));
        }

        var request = new CreateOrderCommand(
            entity.IdCustomer,
            items
        );

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Order", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<OrderModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing order should delete successfully.")]
    public async Task Delete_GivenExistingOrder_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Orders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewOrderAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Order/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}