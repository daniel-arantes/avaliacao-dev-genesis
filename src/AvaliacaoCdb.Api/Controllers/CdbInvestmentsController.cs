using AvaliacaoCdb.Api.Contracts;
using AvaliacaoCdb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AvaliacaoCdb.Api.Controllers;

[ApiController]
[Route("api/investments/cdb")]
public sealed class CdbInvestmentsController(ICdbCalculator calculator) : ControllerBase
{
    [HttpPost("calculate")]
    [ProducesResponseType<CdbCalculationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<CdbCalculationResponse> Calculate(CalculateCdbRequest request)
    {
        var calculation = calculator.Calculate(request.InitialValue, request.Months);
        return Ok(CdbCalculationResponse.FromDomain(calculation));
    }
}
