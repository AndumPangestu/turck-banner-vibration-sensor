using System.Net;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using ReminderManager.Application.Exceptions;
using ReminderManager.Application.Interfaces;
using ReminderManager.Application.Validation;
using ReminderManager.Domain.DTO;
using ReminderManager.Infrastructure.Data;

namespace ReminderManager.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext dbContext, JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> Login(LoginUserRequest request)
        {
            var validator = new LoginUserValidator();
            ValidationResult result = validator.Validate(request);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            var user = await _dbContext.User.FirstOrDefaultAsync(d => d.Username == request.Username);
            if (user == null)
            {
                throw new ResponseException(HttpStatusCode.Unauthorized, "Username or password is wrong");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new ResponseException(HttpStatusCode.Unauthorized, "Username or password is wrong");
            }

            // 🔑 Generate JWT
            var (token, expiration) = _jwtService.GenerateToken(user.Username);

            return user.ToAuthResponse(token, expiration);
        }
    }
}
