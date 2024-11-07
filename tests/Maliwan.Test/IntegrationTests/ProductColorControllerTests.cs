using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.ProductColorCommands;
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
public class ProductColorControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public ProductColorControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get product color by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingProductColor_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductColors
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductColorAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<ProductColorModel?>>($"/ProductColor/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all product colors should get it sucessfully.")]
    public async Task GetAll_GivenExistingProductColor_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductColors
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductColorAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<ProductColorModel>?>>("/ProductColor");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search product colors should at least get a success response code 200/OK")]
    public async Task Search_UserProductColors_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductColors
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductColorAsync();

        var request = new ProductColorSearchRequest(name: entity.Name);

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<ProductColorSearchRequest, BaseResponse<PagedResponse<ProductColorModel>?>>("/ProductColor/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid product color should be created successfully.")]
    public async Task Create_GivenValidProductColor_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = new ProductColorFixture().Valid();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.ProductColors.AnyAsync(e =>
                   (e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   || e.Sku.ToUpper() == entity.Sku.ToUpper())
               && !e.DeletedAt.HasValue))
        {
            entity = new ProductColorFixture().Valid();
            retry--;
        }

        var request = new CreateProductColorCommand(
            entity.Name,
            entity.Sku,
            entity.BgColor,
            entity.TextColor
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/ProductColor", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<ProductColorModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid product color should be updated successfully.")]
    public async Task Update_GivenExistingValidProductColor_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductColors
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewProductColorAsync();

        var request = new UpdateProductColorCommand(
            entity.Id,
            entity.Name,
            entity.Sku,
            entity.BgColor,
            entity.TextColor
            );

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/ProductColor", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<ProductColorModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing product color should delete successfully.")]
    public async Task Delete_GivenExistingProductColor_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.ProductColors
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewProductColorAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/ProductColor/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}