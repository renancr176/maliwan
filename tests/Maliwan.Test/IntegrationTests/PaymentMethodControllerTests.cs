using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.PaymentMethodCommands;
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
public class PaymentMethodControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public PaymentMethodControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get payment method by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingPaymentMethod_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.PaymentMethods
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewPaymentMethodAsync();

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<PaymentMethodModel?>>($"/PaymentMethod/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get all payment methods should get it sucessfully.")]
    public async Task GetAll_GivenExistingPaymentMethod_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.PaymentMethods
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewPaymentMethodAsync();

        if (!await _testsFixture.MaliwanDbContext.PaymentMethods.AnyAsync(e => !e.Active && !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.PaymentMethodFixture.Valid();
            entity.Active = false;
            await _testsFixture.MaliwanDbContext.PaymentMethods.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<BaseResponse<IEnumerable<PaymentMethodModel>?>>("/PaymentMethod");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search payment methods should at least get a success response code 200/OK")]
    public async Task Search_UserPaymentMethods_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.PaymentMethods
                         .FirstOrDefaultAsync(e => e.Active && !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewPaymentMethodAsync();

        var request = new PaymentMethodSearchRequest(name: entity.Name);

        if (!await _testsFixture.MaliwanDbContext.PaymentMethods.AnyAsync(e => !e.Active && !e.DeletedAt.HasValue))
        {
            entity = _testsFixture.EntityFixture.PaymentMethodFixture.Valid();
            entity.Active = false;
            await _testsFixture.MaliwanDbContext.PaymentMethods.AddAsync(entity);
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .RemoveToken()
            .GetFromJsonAsync<PaymentMethodSearchRequest, BaseResponse<PagedResponse<PaymentMethodModel>?>>("/PaymentMethod/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().OnlyContain(e => e.Active);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given valid payment method should be created successfully.")]
    public async Task Create_GivenValidPaymentMethod_ShouldCreateSuccessfully()
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = new PaymentMethodFixture().Valid();
        var retry = 3;
        while (retry > 0 && await _testsFixture.MaliwanDbContext.PaymentMethods.AnyAsync(e =>
                   e.Name.Trim().ToLower() == entity.Name.Trim().ToLower()
               && !e.DeletedAt.HasValue))
        {
            entity = new PaymentMethodFixture().Valid();
            retry--;
        }

        var request = new CreatePaymentMethodCommand(
            entity.Name,
            true
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/PaymentMethod", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<PaymentMethodModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid payment method should be updated successfully.")]
    public async Task Update_GivenExistingValidPaymentMethod_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.PaymentMethods
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
            ?? await _testsFixture.GetInsertedNewPaymentMethodAsync();

        var request = new UpdatePaymentMethodCommand(
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
            .PutAsJsonAsync("/PaymentMethod", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<PaymentMethodModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing payment method should delete successfully.")]
    public async Task Delete_GivenExistingPaymentMethod_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.PaymentMethods
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewPaymentMethodAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/PaymentMethod/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}