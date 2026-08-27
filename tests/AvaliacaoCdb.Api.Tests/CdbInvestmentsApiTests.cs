using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AvaliacaoCdb.Api.Tests;

public sealed class CdbInvestmentsApiTests(WebApplicationFactory<Program> application)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const decimal InvestedAmount = 1_000m;
    private const decimal MonthlyFactor = 1.00972m;

    [Theory]
    [InlineData(6, 0.225)]
    [InlineData(7, 0.20)]
    [InlineData(12, 0.20)]
    [InlineData(13, 0.175)]
    [InlineData(24, 0.175)]
    [InlineData(25, 0.15)]
    public async Task Calculate_ShouldCompoundMonthlyAndApplyTaxBracketBoundaries(
        int months,
        decimal expectedTaxRate)
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/investments/cdb/calculate",
            new { initialValue = InvestedAmount, months });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CdbCalculationResponse>();
        Assert.NotNull(result);

        var grossBeforeRounding = CalculateMonthByMonth(InvestedAmount, months);
        var earningsBeforeRounding = grossBeforeRounding - InvestedAmount;
        var taxBeforeRounding = earningsBeforeRounding * expectedTaxRate;

        Assert.Equal(months, result.Months);
        Assert.Equal(InvestedAmount, result.InitialValue);
        Assert.Equal(expectedTaxRate, result.TaxRate);
        Assert.Equal(Round(grossBeforeRounding), result.GrossAmount);
        Assert.Equal(Round(earningsBeforeRounding), result.GrossEarnings);
        Assert.Equal(Round(taxBeforeRounding), result.TaxAmount);
        Assert.Equal(Round(grossBeforeRounding - taxBeforeRounding), result.NetAmount);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(-1, 2)]
    [InlineData(1_000, 0)]
    [InlineData(1_000, 1)]
    public async Task Calculate_ShouldRejectInvalidInputs(decimal initialValue, int months)
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/investments/cdb/calculate",
            new { initialValue, months });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"initialValue\":1000}")]
    [InlineData("{\"months\":12}")]
    [InlineData("{\"initialValue\":1000,\"months\":12.5}")]
    [InlineData("{invalid-json")]
    public async Task Calculate_ShouldRejectIncompleteOrMalformedJson(string payload)
    {
        using var client = application.CreateClient();
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/investments/cdb/calculate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Calculate_ShouldReturnUnprocessableEntityWhenResultExceedsDecimalCapacity()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/investments/cdb/calculate",
            new { initialValue = decimal.MaxValue, months = 2 });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Não foi possível calcular o investimento.", problem.Title);
    }

    private static decimal CalculateMonthByMonth(decimal value, int months)
    {
        for (var month = 0; month < months; month++)
        {
            value *= MonthlyFactor;
        }
        return value;
    }

    private static decimal Round(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    private sealed record CdbCalculationResponse(
        decimal InitialValue,
        int Months,
        decimal GrossAmount,
        decimal GrossEarnings,
        decimal TaxRate,
        decimal TaxAmount,
        decimal NetAmount);
}
