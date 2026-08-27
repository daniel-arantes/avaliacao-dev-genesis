using AvaliacaoCdb.Domain;

namespace AvaliacaoCdb.Api.Contracts;

public sealed record CdbCalculationResponse(
    decimal InitialValue,
    int Months,
    decimal GrossAmount,
    decimal GrossEarnings,
    decimal TaxRate,
    decimal TaxAmount,
    decimal NetAmount)
{
    public static CdbCalculationResponse FromDomain(CdbCalculation calculation) => new(
        Round(calculation.InitialValue),
        calculation.Months,
        Round(calculation.GrossAmount),
        Round(calculation.GrossEarnings),
        calculation.TaxRate,
        Round(calculation.TaxAmount),
        Round(calculation.NetAmount));

    private static decimal Round(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
