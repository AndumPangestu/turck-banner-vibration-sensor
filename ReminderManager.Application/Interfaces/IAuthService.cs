using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginUserRequest request);
    }
}
