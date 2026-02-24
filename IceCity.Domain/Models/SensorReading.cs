using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Domain.Models
{
    public class SensorReading
    {
        public int SensorReadingId { get; set; }
        public DateTime ReadingDate { get; set; }
        public double WorkingHours { get; set; }
        public double HeaterValue { get; set; }
        public int HeaterId { get; set; }
        public Heater Heater { get; set; }=null!;
    }
}
