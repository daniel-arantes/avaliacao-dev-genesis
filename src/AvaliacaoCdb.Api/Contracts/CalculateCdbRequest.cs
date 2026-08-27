using System.ComponentModel.DataAnnotations;

namespace AvaliacaoCdb.Api.Contracts;

public sealed record CalculateCdbRequest
{
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "O valor inicial deve ser positivo.")]
    public required decimal InitialValue { get; init; }

    [Range(2, int.MaxValue, ErrorMessage = "O prazo deve ser maior que um mes.")]
    public required int Months { get; init; }
}
