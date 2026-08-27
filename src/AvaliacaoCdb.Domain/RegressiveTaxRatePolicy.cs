namespace AvaliacaoCdb.Domain;

public sealed class RegressiveTaxRatePolicy : ITaxRatePolicy
{
    public decimal GetRate(int months)
    {
        if (months <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(months), "O prazo deve ser maior que um mês.");
        }

        return months switch
        {
            <= 6 => 0.225m,
            <= 12 => 0.20m,
            <= 24 => 0.175m,
            _ => 0.15m
        };
    }
}
