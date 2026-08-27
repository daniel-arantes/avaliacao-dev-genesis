namespace AvaliacaoCdb.Domain;

public sealed class CdbCalculator(ITaxRatePolicy taxRatePolicy) : ICdbCalculator
{
    private static decimal CdiMonthlyRate => 0.009m;
    private static decimal CdiPercentagePaidByBank => 1.08m;
    public static decimal MonthlyFactor => 1m + (CdiMonthlyRate * CdiPercentagePaidByBank);

    public CdbCalculation Calculate(decimal initialValue, int months)
    {
        if (initialValue <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(initialValue), "O valor inicial deve ser positivo.");
        }

        if (months <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(months), "O prazo deve ser maior que um mes.");
        }

        var grossAmount = initialValue * Pow(MonthlyFactor, months);
        var grossEarnings = grossAmount - initialValue;
        var taxRate = taxRatePolicy.GetRate(months);
        var taxAmount = grossEarnings * taxRate;

        return new CdbCalculation(
            initialValue,
            months,
            grossAmount,
            grossEarnings,
            taxRate,
            taxAmount,
            grossAmount - taxAmount);
    }

    private static decimal Pow(decimal value, int exponent)
    {
        var result = 1m;
        var factor = value;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result *= factor;
            }

            exponent >>= 1;
            if (exponent > 0)
            {
                factor *= factor;
            }
        }

        return result;
    }
}
