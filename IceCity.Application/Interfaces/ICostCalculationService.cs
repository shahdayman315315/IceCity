using IceCity.Application.Dtos;
using IceCity.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Interfaces
{
    public interface ICostCalculationService
    {
        Task<Result<MonthlyReportDto>> CalculateMonthlyCostAsync(int houseId, CancellationToken cancellationToken = default);
        Task<Result<MonthlyReportDto>> GenerateMonthlyReportAsync(int houseId, CancellationToken cancellationToken = default);
    }
}
