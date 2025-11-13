using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Domain.DTO
{
    public class LoginUserRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public static class AuthMapper
    {
        public static AuthResponse ToAuthResponse(this User user, string token, DateTime expiration)
        {
            return new AuthResponse
            {
                Username = user.Username,
                AccessToken = token,
                Expiration = expiration
            };
        }
    }

    public class AuthResponse
    {
        public required string Username { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? Expiration { get; set; }
    }
}


