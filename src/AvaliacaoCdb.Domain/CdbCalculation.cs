namespace AvaliacaoCdb.Domain;

public sealed record CdbCalculation(
    decimal InitialValue,
    int Months,
    decimal GrossAmount,
    decimal GrossEarnings,
    decimal TaxRate,
    decimal TaxAmount,
    decimal NetAmount);
