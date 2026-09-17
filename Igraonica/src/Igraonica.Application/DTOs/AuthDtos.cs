namespace Igraonica.Application.DTOs;

public sealed record RegisterRequest(
    string Email, string Password, string FirstName, string LastName, string PhoneNumber);

public sealed record LoginRequest(string Email, string Password);

public sealed record TokenResponseDto(
    string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAt, UserDto User);

public sealed record RefreshRequest(string RefreshToken);

public sealed record VerifyEmailRequest(string Token);

public sealed record ResendVerificationRequest(string Email);

public sealed record ForgotPasswordRequest(string Email);

public sealed record ResetPasswordRequest(string Token, string NewPassword);

public sealed record UserDto(
    Guid Id, string Email, string FirstName, string LastName,
    string PhoneNumber, string Role, bool IsEmailVerified);
