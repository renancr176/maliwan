using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.CategoryCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
using Maliwan.Service.Api.Models.Responses;
using Maliwan.Service.Api;
using Maliwan.Test.Extensions;
using Maliwan.Test.Fixtures;
using Maliwan.Test.IntegrationTests.Config;
using Microsoft.EntityFrameworkCore;

namespace Maliwan.Test.IntegrationTests;

[Collection(nameof(IntegrationTestsFixtureCollection))]
public class CategoryControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public CategoryControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get category by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingCategory_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Categories
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewCategoryAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<CategoryModel?>>($"/Category/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all categorys should get it sucessfully.")]
    public async Task GetAll_GivenExistingCategory_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Categories
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewCategoryAsync();

        if (!await _testsFixture.MaliwanDbContext.Categories.AnyAsync(e => !e.Active && !e.DeletedAt.HasValue))
        {
            entity = new EntityFixture().CategoryFixture.Valid();
            entity.Active = false;
            await _testsFixture.MaliwanDbContext.Categories.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<CategoryModel>?>>("/Category");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search categorys should at least get a success response code 200/OK")]
    public async Task Search_UserCategorys_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Categories
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewCategoryAsync();

        var request = new CategorySearchRequest(name: entity.Name);

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<CategorySearchRequest, BaseResponse<PagedResponse<CategoryModel>?>>("/Category/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid category should be created successfully.")]
    public async Task Create_GivenValidCategory_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = new CategoryFixture().Valid();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.Categories.AnyAsync(e =>
                   e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   && !e.DeletedAt.HasValue))
        {
            entity = new CategoryFixture().Valid();
            retry--;
        }

        var request = new CreateCategoryCommand(
            entity.Name,
            true,
            null
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Category", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<CategoryModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid category should be updated successfully.")]
    public async Task Update_GivenExistingValidCategory_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Categories
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewCategoryAsync();

        var request = new UpdateCategoryCommand(
            entity.Id,
            entity.Name,
            !entity.Active);

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/Category", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<CategoryModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing category should delete successfully.")]
    public async Task Delete_GivenExistingCategory_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Categories
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewCategoryAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Category/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}