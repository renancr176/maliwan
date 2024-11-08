using FluentAssertions;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
using Maliwan.Service.Api;
using Maliwan.Service.Api.Models.Responses;
using Maliwan.Test.Extensions;
using Maliwan.Test.Fixtures;
using Maliwan.Test.IntegrationTests.Config;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Maliwan.Application.Commands.MaliwanContext.BrandCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;

namespace Maliwan.Test.IntegrationTests;

[Collection(nameof(IntegrationTestsFixtureCollection))]
public class BrandControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public BrandControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get brand by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingBrand_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Brands
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewBrandAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<BrandModel?>>($"/Brand/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all brands should get it sucessfully.")]
    public async Task GetAll_GivenExistingBrand_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Brands
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewBrandAsync();

        if (!await _testsFixture.MaliwanDbContext.Brands.AnyAsync(e => !e.Active && !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.BrandFixture.Valid();
            entity.Active = false;
            await _testsFixture.MaliwanDbContext.Brands.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<BrandModel>?>>("/Brand");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search brands should at least get a success response code 200/OK")]
    public async Task Search_UserBrands_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Brands
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewBrandAsync();

        var request = new BrandSearchRequest(name: entity.Name);

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BrandSearchRequest, BaseResponse<PagedResponse<BrandModel>?>>("/Brand/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid brand should be created successfully.")]
    public async Task Create_GivenValidBrand_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = _testsFixture.EntityFixture.BrandFixture.Valid();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.Brands.AnyAsync(e => 
                   (e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   || e.Sku.ToUpper() == entity.Sku.ToUpper())
               && !e.DeletedAt.HasValue))
        {
            entity = new BrandFixture().Valid();
            retry--;
        }

        var request = new CreateBrandCommand(
            entity.Name,
            entity.Sku,
            true
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Brand", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<BrandModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid brand should be updated successfully.")]
    public async Task Update_GivenExistingValidBrand_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Brands
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewBrandAsync();

        var request = new UpdateBrandCommand(
            entity.Id,
            entity.Name,
            entity.Sku,
            !entity.Active);

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/Brand", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<BrandModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing brand should delete successfully.")]
    public async Task Delete_GivenExistingBrand_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Brands
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewBrandAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Brand/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}