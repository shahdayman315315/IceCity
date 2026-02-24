using IceC.Domain.Interfaces;
using IceCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<Owner> Owners { get; }
        IBaseRepository<House> Houses { get; }
        IBaseRepository<Heater> Heaters { get; }
        IBaseRepository<MonthlyCostReport> MonthlyCostReports { get; }
        IBaseRepository<SensorReading> SensorReadings { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
