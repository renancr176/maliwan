using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.StockCommands;
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
public class StockControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public StockControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get stock by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingStock_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.GetInsertedNewStockAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<BaseResponse<StockModel?>>($"/Stock/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Theory(DisplayName = "Search stocks should at least get a success response code 200/OK")]
    [InlineData("ByProduct", null)]
    [InlineData("CurrentQuantity", null)]
    [InlineData("StockLevel", StockLevelEnum.High)]
    [InlineData("StockLevel", StockLevelEnum.Medium)]
    [InlineData("StockLevel", StockLevelEnum.Low)]
    public async Task Search_UserStocks_ShouldQuerySuccessfully(string testType, StockLevelEnum? stockLevel = StockLevelEnum.High)
    {
        // Arrange 
        var entity = await _testsFixture.GetInsertedNewStockAsync();

        StockSearchRequest request = new StockSearchRequest();

        switch (testType)
        {
            case "ByProduct":
                request = new StockSearchRequest(idProduct: entity.IdProduct);
                break;
            case "CurrentQuantity":
                await _testsFixture.GetInsertedNewOrderAsync(stocks: new List<Stock>() { entity });
                entity = await _testsFixture.MaliwanDbContext.Stocks.Include(nameof(Stock.OrderItems))
                    .FirstOrDefaultAsync(e => e.Id == entity.Id);
                request = new StockSearchRequest(
                    currentQuantityMin: (entity.CurrentQuantity - 1) > 0 ? (entity.CurrentQuantity - 1) : 0,
                    currentQuantityMax: (entity.CurrentQuantity + 1));
                break;
            case "StockLevel":
                var order = await _testsFixture.GetInsertedNewOrderAsync(stocks: new List<Stock>() { entity });
                entity.InputQuantity = 9;
                _testsFixture.MaliwanDbContext.Stocks.Update(entity);
                await _testsFixture.MaliwanDbContext.SaveChangesAsync();

                switch (stockLevel)
                {
                    case StockLevelEnum.Low:
                        
                        if (entity.StockLevel != StockLevelEnum.Low)
                        {
                            var orderItem = await _testsFixture.MaliwanDbContext.OrderItems.FirstOrDefaultAsync(e =>
                                e.Id == order.OrderItems.First().Id);
                            orderItem.Quantity = 6;
                            _testsFixture.MaliwanDbContext.OrderItems.Update(orderItem);
                            await _testsFixture.MaliwanDbContext.SaveChangesAsync();
                        }
                        break;
                    case StockLevelEnum.Medium:
                        if (entity.StockLevel != StockLevelEnum.Medium)
                        {
                            var orderItem = await _testsFixture.MaliwanDbContext.OrderItems.FirstOrDefaultAsync(e =>
                                e.Id == order.OrderItems.First().Id);
                            orderItem.Quantity = 4;
                            _testsFixture.MaliwanDbContext.OrderItems.Update(orderItem);
                            await _testsFixture.MaliwanDbContext.SaveChangesAsync();
                        }
                        break;
                }

                request = new StockSearchRequest(stockLevel: stockLevel);
                break;
        }

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<StockSearchRequest, BaseResponse<PagedResponse<StockModel>?>>("/Stock/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
        if (testType == "CurrentQuantity")
            responseObj.Data.Data.Should().OnlyContain(e => e.CurrentQuantity >= request.CurrentQuantityMin && e.CurrentQuantity <= request.CurrentQuantityMax);
        if (testType == "StockLevel")
            responseObj.Data.Data.Should().OnlyContain(e => e.StockLevel == stockLevel);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid stock should be created successfully.")]
    public async Task Create_GivenValidStock_ShouldCreateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.GetNewStockAsync();

        var request = new CreateStockCommand(
            entity.IdProduct,
            entity.InputQuantity,
            entity.InputDate,
            entity.PurchasePrice,
            entity.Active,
            entity.IdSize,
            entity.IdColor
            );

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Stock", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<StockModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid stock should be updated successfully.")]
    public async Task Update_GivenExistingValidStock_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Stocks
                         .FirstOrDefaultAsync(e =>
                             !e.DeletedAt.HasValue &&
                             e.Color.DeletedAt.HasValue &&
                             e.Size.DeletedAt.HasValue &&
                             !e.Product.DeletedAt.HasValue &&
                             !e.Product.Brand.DeletedAt.HasValue &&
                             !e.Product.Gender.DeletedAt.HasValue &&
                             !e.Product.Subcategory.DeletedAt.HasValue &&
                             !e.Product.Subcategory.Category.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewStockAsync();

        var request = new UpdateStockCommand(
            entity.Id,
            entity.InputQuantity,
            entity.InputDate,
            entity.PurchasePrice,
            entity.Active,
            entity.IdSize,
            entity.IdColor
        );

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/Stock", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<StockModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing stock should delete successfully.")]
    public async Task Delete_GivenExistingStock_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Stocks
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewStockAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Stock/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}