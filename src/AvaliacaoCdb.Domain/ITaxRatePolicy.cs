namespace AvaliacaoCdb.Domain;

public interface ITaxRatePolicy
{
    decimal GetRate(int months);
}
