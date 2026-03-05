using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IceCity.Application.Dtos
{
    public class AuthDto
    {
        public string UserName { get; set; }=null!;
        public string Role { get; set; }=null!;
        public string Token { get; set; }=null!;
        public DateTime ExpirationDate { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }=null!;
        public DateTime RefreshTokenExpiration { get; set; }
        public string Message { get; set; }=null!;
        public bool IsAuthenticated { get; set; }
    }
}
