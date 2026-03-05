using IceCity.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthDto> LoginAsync(LoginDto loginDto);
        Task<AuthDto> RefreshTokenAsync(RefreshTokenDto dto);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

    }
}
