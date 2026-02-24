using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Dtos
{
    public class MonthlyReportDto
    {
        public int HouseId { get; set; }

        public string ReportMonth { get; set; }=null!;

        public double TotalWorkingHours { get; set; }

        public double MedianHeaterValue { get; set; }

        public double MonthlyAverageCost { get; set; }
    }

}
