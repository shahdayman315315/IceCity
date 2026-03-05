using IceCity.Application.Dtos;
using IceCity.Application.Interfaces;
using IceCity.Domain.Interfaces;
using IceCity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUserName = await _unitOfWork.Users.Query.FirstOrDefaultAsync(u=>u.Username==registerDto.UserName);

            if(existingUserName is not null)
            {
                return new AuthDto { Message = "Username already exists" };
            }

            var existingEmail = await _unitOfWork.Users.Query.FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if(existingEmail is not null)
            {
                return new AuthDto { Message = "Email already exists" };
            }

            var user = new User
            {
                Username = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow
            };

                
    
                var token = _tokenService.GenerateToken(user);

                var refreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.UserId,
                    ExpiresOn = DateTime.UtcNow.AddDays(7),
                    CreatedOn = DateTime.UtcNow,

                };

            user.RefreshTokens.Add(refreshTokenEntity);
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IsAuthenticated = true,
                Message = "Registration successful",
                Role = user.Role,
                UserName = user.Username,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenEntity.ExpiresOn,
                ExpirationDate = token.ValidTo
            };
        }
        public async Task<AuthDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.Query.Include(u=>u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == loginDto.Email);


            if(user is null ||!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return new AuthDto { Message = "Invalid Email or password" };
            }

            var token = _tokenService.GenerateToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.UserId,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow,

            };

            var expiredTokens = user.RefreshTokens.Where(t => t.IsExpired).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                user.RefreshTokens.Remove(expiredToken);
            }

            user.RefreshTokens.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IsAuthenticated = true,
                Message = "Login successful",
                Role = user.Role,
                UserName = user.Username,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenEntity.ExpiresOn,
                ExpirationDate = token.ValidTo 
            };
        }

        public async Task<AuthDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            if (principal is null)
            {
                return new AuthDto { Message = "Invalid access token" };
            }

            var userId=principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user= await _unitOfWork.Users.Query.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
           
            if (user is null)
            {
                return new AuthDto { Message = "User not found" };
            }

            var storedRefreshToken =  user.RefreshTokens.FirstOrDefault(t => t.Token == dto.RefreshToken);

            if(storedRefreshToken is null || !storedRefreshToken.IsActive)
            {
                return new AuthDto { Message = "Invalid or expired refresh token" };
            }

            storedRefreshToken.RevokedOn = DateTime.UtcNow;

            var jwtToken = _tokenService.GenerateToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.UserId,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow,
            };

            user.RefreshTokens.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                IsAuthenticated = true,
                Message = "Token refreshed successfully",
                Role = user.Role,
                UserName = user.Username,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenEntity.ExpiresOn,
                ExpirationDate = jwtToken.ValidTo
            };

        }


        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _unitOfWork.RefreshTokens.Query
             .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (existRefreshToken is null || !existRefreshToken.IsActive)
                return false;

            existRefreshToken.RevokedOn = DateTime.UtcNow;

            _unitOfWork.RefreshTokens.UpdateAsync(existRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        
    }
}
