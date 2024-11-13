using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.StockCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
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
        var entity = await _testsFixture.MaliwanDbContext.Stocks
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewStockAsync();

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
    [InlineData("ByProduct")]
    [InlineData("CurrentQuantity")]
    //[InlineData("StockLevel")]
    public async Task Search_UserStocks_ShouldQuerySuccessfully(string testType)
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Stocks
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue && !e.OrderItems.Any())
                     ?? await _testsFixture.GetInsertedNewStockAsync();

        StockSearchRequest request = new StockSearchRequest();
        var stockLevel = _testsFixture.EntityFixture.Faker.PickRandom<StockLevelEnum>();

        switch (testType)
        {
            case "ByProduct":
                request = new StockSearchRequest(idProduct: entity.IdProduct);
                break;
            case "CurrentQuantity":
                //TODO: Criar pedido com este item para decrementar a quantidade atual.
                request = new StockSearchRequest(
                    currentQuantityMin: (entity.InputQuantity - 1) > 0 ? (entity.InputQuantity - 1) : 0,
                    currentQuantityMax: (entity.InputQuantity + 1));
                break;
            //case "StockLevel":
            //    request = new StockSearchRequest(stockLevel: stockLevel);
            //    break;
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
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
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