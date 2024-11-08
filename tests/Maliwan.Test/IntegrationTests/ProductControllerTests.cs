using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.ProductCommands;
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
public class ProductControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public ProductControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get product by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingProduct_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Products
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<ProductModel?>>($"/Product/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search products should at least get a success response code 200/OK")]
    public async Task Search_UserProducts_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Products
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductAsync();

        var request = new ProductSearchRequest(idSubcategory: entity.IdSubcategory);

        if (!await _testsFixture.MaliwanDbContext.Products.AnyAsync(e => !e.Active && !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.ProductFixture.Valid();
            entity.Active = false;
            await _testsFixture.MaliwanDbContext.Products.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<ProductSearchRequest, BaseResponse<PagedResponse<ProductModel>?>>("/Product/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid product should be created successfully.")]
    public async Task Create_GivenValidProduct_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = await _testsFixture.GetNewProductAsync();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.Products.AnyAsync(e =>
                   (e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   || e.Sku.ToUpper() == entity.Sku.ToUpper())
                   && e.IdBrand == entity.IdBrand
                   && e.IdSubcategory == entity.IdSubcategory
                   && e.IdGender == entity.IdGender
                   && !e.DeletedAt.HasValue))
        {
            entity = await _testsFixture.GetNewProductAsync();
            retry--;
        }

        var request = new CreateProductCommand(
            entity.IdBrand,
            entity.IdSubcategory,
            entity.Name,
            entity.UnitPrice,
            entity.Sku,
            entity.Active,
            entity.IdGender
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Product", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<ProductModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid product should be updated successfully.")]
    public async Task Update_GivenExistingValidProduct_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Products
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewProductAsync();

        var request = new UpdateProductCommand(
            entity.Id,
            entity.IdBrand,
            entity.IdSubcategory,
            entity.Name,
            entity.UnitPrice,
            entity.Sku,
            !entity.Active,
            entity.IdGender);

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/Product", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<ProductModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing product should delete successfully.")]
    public async Task Delete_GivenExistingProduct_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Products
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Product/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}