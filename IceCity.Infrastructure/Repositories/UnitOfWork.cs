using IceC.Domain.Interfaces;
using IceCity.Domain.Interfaces;
using IceCity.Domain.Models;
using IceCity.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private Lazy<IBaseRepository<Owner>> _owners;
        private Lazy<IBaseRepository<House>> _houses;
        private Lazy<IBaseRepository<Heater>> _heaters;
        private Lazy<IBaseRepository<MonthlyCostReport>> _monthlyCostReports;
        private Lazy<IBaseRepository<SensorReading>> _sensorReadings;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _owners = CreateRepository<IBaseRepository<Owner>, BaseRepository<Owner>>();
            _houses = CreateRepository<IBaseRepository<House>, BaseRepository<House>>();
            _heaters = CreateRepository<IBaseRepository<Heater>, BaseRepository<Heater>>();
            _monthlyCostReports = CreateRepository<IBaseRepository<MonthlyCostReport>, BaseRepository<MonthlyCostReport>>();
            _sensorReadings = CreateRepository<IBaseRepository<SensorReading>, BaseRepository<SensorReading>>();

        }

        private Lazy<T1> CreateRepository<T1, T2>() where T1 : class where T2 : class
        {
            return new Lazy<T1>(() => (T1)Activator.CreateInstance(typeof(T2), _context)!);
        }
        public IBaseRepository<Owner> Owners => _owners.Value;

        public IBaseRepository<House> Houses => _houses.Value;

        public IBaseRepository<Heater> Heaters => _heaters.Value;

        public IBaseRepository<MonthlyCostReport> MonthlyCostReports => _monthlyCostReports.Value;

        public IBaseRepository<SensorReading> SensorReadings => _sensorReadings.Value;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
