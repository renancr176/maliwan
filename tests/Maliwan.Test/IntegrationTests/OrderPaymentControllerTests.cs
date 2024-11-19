using System.Net.Http.Json;
using FluentAssertions;
using Maliwan.Application.Commands.MaliwanContext.OrderPaymentCommands;
using Maliwan.Application.Models.MaliwanContext.Queries.Requests;
using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Responses;
using Maliwan.Domain.MaliwanContext.Entities;
using Maliwan.Service.Api;
using Maliwan.Service.Api.Models.Responses;
using Maliwan.Test.Extensions;
using Maliwan.Test.IntegrationTests.Config;
using Microsoft.EntityFrameworkCore;

namespace Maliwan.Test.IntegrationTests;

[Collection(nameof(IntegrationTestsFixtureCollection))]
public class OrderPaymentControllerTests
{
    private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

    public OrderPaymentControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    #region Negative Cases



    #endregion

    #region Positive Cases

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get order payment by Id should get it sucessfully.")]
    public async Task GetById_GivenExistingOrderPayment_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.OrderPayments
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewOrderPaymentAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<BaseResponse<OrderPaymentModel?>>($"/OrderPayment/{entity.Id}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Get order payment by IdOrder should get it sucessfully.")]
    public async Task GetByIdOrder_GivenExistingOrderPayment_ShouldGetSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.OrderPayments
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewOrderPaymentAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<BaseResponse<IEnumerable<OrderPaymentModel>?>>($"/OrderPayment/Order/{entity.IdOrder}");

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Search order payments should at least get a success response code 200/OK")]
    public async Task Search_UserOrderPayments_ShouldQuerySuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.OrderPayments
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewOrderPaymentAsync();

        var request = new OrderPaymentSearchRequest(paymentDate: entity.PaymentDate);

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act & Assert
        var responseObj = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .GetFromJsonAsync<OrderPaymentSearchRequest, BaseResponse<PagedResponse<OrderPaymentModel>?>>("/OrderPayment/Search", request);

        // Assert 
        responseObj.Should().NotBeNull();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
        responseObj.Data.Should().NotBeNull();
        responseObj.Data.Data.Should().NotBeNull();
    }

    [Trait("IntegrationTest", "Controllers")]
    [Theory(DisplayName = "Given valid order payment should be created successfully.")]
    [InlineData("PayFullAmount")]
    [InlineData("PayPartialAmount")]
    public async Task Create_GivenValidOrderPayment_ShouldCreateSuccessfully(string testType)
    {
        // Arrange 
        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        var entity = await _testsFixture.GetNewOrderPaymentAsync();

        var request = new CreateOrderPaymentCommand(
            entity.IdOrder,
            entity.IdPaymentMethod,
            testType == "PayPartialAmount" 
                ? decimal.Round((entity.AmountPaid / 2), 2, MidpointRounding.AwayFromZero) 
                : entity.AmountPaid
            );

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PostAsJsonAsync("/OrderPayment", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<OrderPaymentModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);

        var order = await _testsFixture.MaliwanDbContext.Orders
            .Include(nameof(Order.OrderItems))
            .Include(nameof(Order.OrderPayments))
            .FirstOrDefaultAsync(e => e.Id == entity.IdOrder);
        switch (testType)
        {
            case "PayFullAmount":
                order.OutstandingBalance.Should().Be(0M);
                break;
            case "PayPartialAmount":
                order.OutstandingBalance.Should().Be((entity.AmountPaid - request.AmountPaid));
                break;
        }
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing valid orderPayment should be updated successfully.")]
    public async Task Update_GivenExistingValidOrderPayment_ShouldUpdateSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.GetInsertedNewOrderPaymentAsync();

        var request = new UpdateOrderPaymentCommand(
            entity.Id,
            decimal.Round((entity.AmountPaid / 2), 2, MidpointRounding.AwayFromZero));

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .PutAsJsonAsync("/OrderPayment", request);

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse<OrderPaymentModel?>>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    [Trait("IntegrationTest", "Controllers")]
    [Fact(DisplayName = "Given existing orderPayment should delete successfully.")]
    public async Task Delete_GivenExistingOrderPayment_ShouldDeleteSuccessfully()
    {
        // Arrange 
        var entity = await _testsFixture.MaliwanDbContext.OrderPayments
                         .FirstOrDefaultAsync(e => !e.DeletedAt.HasValue)
                     ?? await _testsFixture.GetInsertedNewOrderPaymentAsync();

        if (string.IsNullOrEmpty(_testsFixture.AdminAccessToken))
        {
            await _testsFixture.AuthenticateAsAdminAsync();
        }

        // Act 
        var response = await _testsFixture.Client
            .AddToken(_testsFixture.AdminAccessToken)
            .DeleteAsync($"/OrderPayment/{entity.Id}");

        // Assert 
        response.EnsureSuccessStatusCode();
        var responseObj = await response.DeserializeObject<BaseResponse>();
        responseObj.Success.Should().BeTrue();
        responseObj.Errors.Should().HaveCount(0);
    }

    #endregion
}