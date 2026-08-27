using AvaliacaoCdb.Domain;

namespace AvaliacaoCdb.Domain.Tests;

public sealed class CdbCalculatorTests
{
    private readonly CdbCalculator _calculator = new(new RegressiveTaxRatePolicy());

    [Fact]
    public void Calculate_ShouldCompoundMonthlyAndTaxOnlyEarnings()
    {
        var result = _calculator.Calculate(1_000m, 2);

        Assert.Equal(1_000m, result.InitialValue);
        Assert.Equal(2, result.Months);
        Assert.Equal(1_019.5344784m, result.GrossAmount);
        Assert.Equal(19.5344784m, result.GrossEarnings);
        Assert.Equal(0.225m, result.TaxRate);
        Assert.Equal(4.39525764m, result.TaxAmount);
        Assert.Equal(1_015.13922076m, result.NetAmount);
    }

    [Fact]
    public void Calculate_ShouldPreserveFractionalInitialValue()
    {
        var result = _calculator.Calculate(10.25m, 2);

        Assert.Equal(10.25m * CdbCalculator.MonthlyFactor * CdbCalculator.MonthlyFactor, result.GrossAmount);
        Assert.True(result.NetAmount > result.InitialValue);
        Assert.True(result.NetAmount < result.GrossAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Calculate_ShouldRejectNonPositiveInitialValue(decimal initialValue)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Calculate(initialValue, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Calculate_ShouldRejectTermsShorterThanTwoMonths(int months)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Calculate(1_000m, months));
    }
}
