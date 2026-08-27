namespace AvaliacaoCdb.Domain;

public interface ICdbCalculator
{
    CdbCalculation Calculate(decimal initialValue, int months);
}
