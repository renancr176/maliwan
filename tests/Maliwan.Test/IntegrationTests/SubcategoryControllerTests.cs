using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;
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
public class SubcategoryControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public SubcategoryControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get subcategory by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingSubcategory_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Subcategories
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewSubcategoryAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<SubcategoryModel?>>($"/Subcategory/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all subcategorys should get it sucessfully.")]
    public async Task GetAll_GivenExistingSubcategory_ShouldGetSuccessfully()
    {
        // Arrange 
        var category = await _testsFixture.MaliwanDbContext.Categories
                           .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                       ?? await _testsFixture.GetInsertedNewCategoryAsync();
        var entity = await _testsFixture.MaliwanDbContext.Subcategories
            .FirstOrDefaultAsync(e => e.IdCategory == category.Id && e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewSubcategoryAsync(category);

        if (!await _testsFixture.MaliwanDbContext.Subcategories.AnyAsync(e => !e.Active && !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.SubcategoryFixture.Valid();
            entity.IdCategory = category.Id;
            entity.Active = false;
            await _testsFixture.MaliwanDbContext.Subcategories.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<SubcategoryModel>?>>("/Subcategory");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search subcategorys should at least get a success response code 200/OK")]
    public async Task Search_UserSubcategorys_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Subcategories
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewSubcategoryAsync();

        var request = new SubcategorySearchRequest(name: entity.Name);

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<SubcategorySearchRequest, BaseResponse<PagedResponse<SubcategoryModel>?>>("/Subcategory/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid subcategory should be created successfully.")]
    public async Task Create_GivenValidSubcategory_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var category = await _testsFixture.MaliwanDbContext.Categories
                           .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                       ?? await _testsFixture.GetInsertedNewCategoryAsync();
        var entity = _testsFixture.EntityFixture.SubcategoryFixture.Valid();
        entity.IdCategory = category.Id;
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.Subcategories.AnyAsync(e =>
                   e.IdCategory == category.Id &&
                   (e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   || e.Sku.ToUpper() == entity.Sku.ToUpper())
               && !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.SubcategoryFixture.Valid();
            entity.IdCategory = category.Id;
            retry--;
        }

        var request = new CreateSubcategoryCommand(
            entity.IdCategory,
            entity.Name,
            entity.Sku,
            true
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Subcategory", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<SubcategoryModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid subcategory should be updated successfully.")]
    public async Task Update_GivenExistingValidSubcategory_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Subcategories
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewSubcategoryAsync();

        var request = new UpdateSubcategoryCommand(
            entity.Id,
            entity.IdCategory,
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
            .PutAsJsonAsync("/Subcategory", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<SubcategoryModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing subcategory should delete successfully.")]
    public async Task Delete_GivenExistingSubcategory_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Subcategories
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewSubcategoryAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Subcategory/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}