using Igraonica.Application.DTOs;

namespace Igraonica.Application.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<TokenResponseDto> LoginAsync(LoginRequest request, CancellationToken ct);
    Task VerifyEmailAsync(string token, CancellationToken ct);
    Task ResendVerificationAsync(string email, CancellationToken ct);
    Task<TokenResponseDto> RefreshAsync(string refreshToken, CancellationToken ct);
    Task ForgotPasswordAsync(string email, CancellationToken ct);
    Task ResetPasswordAsync(string token, string newPassword, CancellationToken ct);
}
