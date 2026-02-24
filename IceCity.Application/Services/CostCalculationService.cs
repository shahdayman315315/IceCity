using AutoMapper;
using IceCity.Application.Dtos;
using IceCity.Application.Helpers;
using IceCity.Application.Interfaces;
using IceCity.Domain.Interfaces;
using IceCity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Services
{
    public class CostCalculationService : ICostCalculationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CostCalculationService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<MonthlyReportDto>> CalculateMonthlyCostAsync(int houseId, CancellationToken cancellationToken = default)
        {
            var house = await _unitOfWork.Houses.GetByIdAsync(houseId,cancellationToken);

            if ( house is null)
            {
                return Result<MonthlyReportDto>.Failure("House not found");
            }

            var readings=await _unitOfWork.SensorReadings.Query
                .Where(r => r.Heater.HouseId == houseId && r.ReadingDate >= DateTime.UtcNow.AddDays(-30))
                .ToListAsync(cancellationToken);

            if(readings.Count == 0)
            {
                return Result<MonthlyReportDto>.Success(new MonthlyReportDto { MonthlyAverageCost = 0 });
            }

            var totalWorkingHours =  readings.Sum(r => r.WorkingHours);

            var medianHeaterValue= CalculateMedian(readings.Select(r => r.HeaterValue).ToList(),cancellationToken);

            var totalCost = medianHeaterValue * (totalWorkingHours / (24 * 30));


            var report = new MonthlyCostReport
            {
                HouseId = houseId,
                ReportMonth = DateTime.UtcNow.Month,
                MonthlyAverageCost = totalCost,
                TotalWorkingHours = totalWorkingHours,
                MedianHeaterValue = medianHeaterValue,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MonthlyCostReports.AddAsync(report,cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto=_mapper.Map<MonthlyReportDto>(report);

            return Result<MonthlyReportDto>.Success(dto);

        }

        public async Task<Result<MonthlyReportDto>> GenerateMonthlyReportAsync(int houseId, CancellationToken cancellationToken = default)
        {
            var house = await _unitOfWork.Houses.GetByIdAsync(houseId, cancellationToken);

            if (house is null)
            {
                return Result<MonthlyReportDto>.Failure("House not found");
            }

            var existingReport = await _unitOfWork.MonthlyCostReports.Query
                .Where(r => r.HouseId == houseId && r.CreatedAt.Month == DateTime.UtcNow.Month&& r.CreatedAt.Year==DateTime.UtcNow.Year)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingReport is not null)
            {
                var dto = _mapper.Map<MonthlyReportDto>(existingReport);

                return Result<MonthlyReportDto>.Success(dto);
            }


            return  await CalculateMonthlyCostAsync(houseId, cancellationToken);

           

        }

        private double CalculateMedian(List<double> numbers,CancellationToken cancellationToken)
        {
            var sortedNumbers = numbers.OrderBy(n => n).ToList();
            int count = sortedNumbers.Count;
            if (count % 2 == 0)
            {
                // لو العدد زوجي نأخذ متوسط الرقمين اللي في النص
                return (sortedNumbers[count / 2 - 1] + sortedNumbers[count / 2]) / 2;
            }
            else
            {
                // لو العدد فردي نأخذ الرقم اللي في النص بالضبط
                return sortedNumbers[count / 2];
            }
        }
    }
}
