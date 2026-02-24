using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Domain.Models
{
    public class Heater
    {
        public int HeaterId { get; set; }
        public string HeaterType { get; set; }=null!;
        public double PowerValue { get; set; }
        public int HouseId { get; set; }
        public House House { get; set; }=null!;
        public ICollection<SensorReading> SensorReadings { get; set; }=new List<SensorReading>();
    }
}
