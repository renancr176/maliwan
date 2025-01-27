﻿using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.GenderCommands;
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
public class GenderControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public GenderControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get gender by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingGender_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Genders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewGenderAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<GenderModel?>>($"/Gender/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all genders should get it sucessfully.")]
    public async Task GetAll_GivenExistingGender_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Genders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewGenderAsync();

        if (!await _testsFixture.MaliwanDbContext.Genders.AnyAsync(e => !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.GenderFixture.Valid();
            await _testsFixture.MaliwanDbContext.Genders.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<GenderModel>?>>("/Gender");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search genders should at least get a success response code 200/OK")]
    public async Task Search_UserGenders_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Genders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewGenderAsync();

        var request = new GenderSearchRequest(name: entity.Name);

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<GenderSearchRequest, BaseResponse<PagedResponse<GenderModel>?>>("/Gender/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid gender should be created successfully.")]
    public async Task Create_GivenValidGender_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = new GenderFixture().Valid();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.Genders.AnyAsync(e =>
                   (e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
                   || e.Sku.ToUpper() == entity.Sku.ToUpper())
               && !e.DeletedAt.HasValue))
        {
            entity = new GenderFixture().Valid();
            retry--;
        }

        var request = new CreateGenderCommand(
            entity.Name,
            entity.Sku
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/Gender", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<GenderModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid gender should be updated successfully.")]
    public async Task Update_GivenExistingValidGender_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.Genders
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewGenderAsync();

        var request = new UpdateGenderCommand(
            entity.Id,
            entity.Name,
            entity.Sku);

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/Gender", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<GenderModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing gender should delete successfully.")]
    public async Task Delete_GivenExistingGender_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.GetInsertedNewGenderAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/Gender/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}