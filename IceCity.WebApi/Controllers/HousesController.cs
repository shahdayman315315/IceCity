using IceCity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceCity.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly ICostCalculationService _costCalculationService;
        public HousesController(ICostCalculationService costCalculationService)
        {
            _costCalculationService = costCalculationService;
        }


        [HttpGet("{houseid}/cost")]
        public async Task<IActionResult> CalculateCost(int houseid, CancellationToken cancellationToken)
        {
            var result = await _costCalculationService.CalculateMonthlyCostAsync(houseid, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Cost = result.Data!.MonthlyAverageCost });
        }

        [HttpGet("{houseid}/monthly-report")]
        public async Task<IActionResult> GenerateMonthlyReport(int houseid, CancellationToken cancellationToken)
        {
            var result = await _costCalculationService.GenerateMonthlyReportAsync(houseid, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
