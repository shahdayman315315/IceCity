using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Domain.Models
{
    public class MonthlyCostReport
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public House House { get; set; }=null!;
        public int ReportMonth { get; set; }
        public double TotalWorkingHours { get; set; }
        public double MedianHeaterValue { get; set; }
        public double MonthlyAverageCost { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
