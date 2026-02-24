using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Domain.Models
{
    public class House
    {
        public int HouseId { get; set; }
        public string Address { get; set; }=null!;
        public string CityZone { get; set; }=null!;
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }=null!;
        public ICollection<Heater> Heaters { get; set; }=new List<Heater>();
        public ICollection<MonthlyCostReport> MonthlyCostReports { get; set; } = new List<MonthlyCostReport>();
    }
}
