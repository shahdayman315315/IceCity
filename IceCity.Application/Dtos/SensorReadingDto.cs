using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Dtos
{
    public class SensorReadingDto
    {
        [Required]
        public DateTime ReadingDate { get; set; }

        [Required,Range(1,24)]
        public double WorkingHours { get; set; }

        [Required]
        public double HeaterValue { get; set; }

        [Required]
        public int HeaterId { get; set; }
    }
}
