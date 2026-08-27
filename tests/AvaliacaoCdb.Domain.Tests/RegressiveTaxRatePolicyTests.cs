using AvaliacaoCdb.Domain;

namespace AvaliacaoCdb.Domain.Tests;

public sealed class RegressiveTaxRatePolicyTests
{
    private readonly RegressiveTaxRatePolicy _policy = new();

    [Theory]
    [InlineData(2, 0.225)]
    [InlineData(6, 0.225)]
    [InlineData(7, 0.20)]
    [InlineData(12, 0.20)]
    [InlineData(13, 0.175)]
    [InlineData(24, 0.175)]
    [InlineData(25, 0.15)]
    [InlineData(1200, 0.15)]
    public void GetRate_ShouldUseRegressiveBoundaries(int months, decimal expected)
    {
        Assert.Equal(expected, _policy.GetRate(months));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void GetRate_ShouldRejectInvalidTerm(int months)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _policy.GetRate(months));
    }
}
