using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.ProductSizeCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
using Maliwan.Service.Api;
using Maliwan.Service.Api.Models.Responses;
using Maliwan.Test.Extensions;
using Maliwan.Test.Fixtures;
using Maliwan.Test.IntegrationTests.Config;
using Microsoft.EntityFrameworkCore;

namespace Maliwan.Test.IntegrationTests;

[Collection(nameof(IntegrationTestsFixtureCollection))]
public class ProductSizeControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public ProductSizeControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get product size by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingProductSize_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductSizes
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductSizeAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<ProductSizeModel?>>($"/ProductSize/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all product sizes should get it sucessfully.")]
    public async Task GetAll_GivenExistingProductSize_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductSizes
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductSizeAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<ProductSizeModel>?>>("/ProductSize");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search product sizes should at least get a success response code 200/OK")]
    public async Task Search_UserProductSizes_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductSizes
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductSizeAsync();

        var request = new ProductSizeSearchRequest(name: entity.Name);

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<ProductSizeSearchRequest, BaseResponse<PagedResponse<ProductSizeModel>?>>("/ProductSize/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid product size should be created successfully.")]
    public async Task Create_GivenValidProductSize_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = new ProductSizeFixture().Valid();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.ProductSizes.AnyAsync(e =>
                   (e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   || e.Sku.ToUpper() == entity.Sku.ToUpper())
               && !e.DeletedAt.HasValue))
        {
            entity = new ProductSizeFixture().Valid();
            retry--;
        }

        var request = new CreateProductSizeCommand(
            entity.Name,
            entity.Sku
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/ProductSize", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<ProductSizeModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid product size should be updated successfully.")]
    public async Task Update_GivenExistingValidProductSize_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductSizes
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewProductSizeAsync();

        var request = new UpdateProductSizeCommand(
            entity.Id,
            entity.Name,
            entity.Sku
            );

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/ProductSize", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<ProductSizeModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing product size should delete successfully.")]
    public async Task Delete_GivenExistingProductSize_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductSizes
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductSizeAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/ProductSize/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}