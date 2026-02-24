using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Domain.Models
{
    public class Owner
    {
        public int OwnerId { get; set; }    
        public string FullName { get; set; }=null!;
        public string PhoneNumber { get; set; }=null!;
        public string Email { get; set; }=null!;
        public ICollection<House> Houses { get; set; }=new List<House>();
    }
}
