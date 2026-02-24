using AutoMapper;
using IceCity.Application.Dtos;
using IceCity.Domain.Interfaces;
using IceCity.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceCity.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SensorsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpPost("readings")]
        public async Task<IActionResult> EnterReadings(SensorReadingDto dto)
        {
            var heater = await _unitOfWork.Heaters.GetByIdAsync(dto.HeaterId);

            if(heater is null)
            {
                return BadRequest("Heater not found");
            }

            if(dto.ReadingDate > DateTime.UtcNow)
            {
                return BadRequest("Reading date cannot be in the future");
            }

            var sensorReading = _mapper.Map<SensorReading>(dto);

            await _unitOfWork.SensorReadings.AddAsync(sensorReading);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Sensor reading added successfully");
        }
    }
}
