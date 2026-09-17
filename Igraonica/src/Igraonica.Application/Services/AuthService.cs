using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Emails;
using Igraonica.Application.Mappings;
using Igraonica.Application.Options;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Igraonica.Application.Services;

public class AuthService(
    IUserRepository _userRepository,
    IRefreshTokenRepository _refreshTokenRepository,
    IOutboxRepository _outbox,
    IUnitOfWork _unitOfWork,
    IPasswordHasher _passwordHasher,
    IJwtService _jwtService,
    IDateTimeProvider _clock,
    IOptions<JwtOptions> _jwtOptions,
    IOptions<ClientOptions> _clientOptions) : IAuthService
{
    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await _userRepository.EmailExistsAsync(request.Email, ct))
            throw new DuplicateEmailException("Nalog sa ovom adresom elektronske poste vec postoji.");

        var verificationToken = Guid.NewGuid().ToString("N");
        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Register(request.Email, passwordHash, request.FirstName,
            request.LastName, request.PhoneNumber, verificationToken, _clock.UtcNow);

        await _userRepository.AddAsync(user, ct);

        var verifyUrl = $"{_clientOptions.Value.BaseUrl}/verify-email?token={verificationToken}";
        var (subject, body) = EmailTemplates.EmailVerification(user.FirstName, verifyUrl);
        await _outbox.AddAsync(
            OutboxMessage.Create(OutboxMessageType.EmailVerification, user.Email, subject, body, _clock.UtcNow), ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return user.ToDto();
    }

    public async Task<TokenResponseDto> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);

        // Namerno ista poruka bez obzira na to da li adresa postoji ili je lozinka
        // pogresna, kako se ne bi otkrilo postojanje naloga (Tabela 9).
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException("Pogresna adresa elektronske poste ili lozinka.");

        return await IssueTokensAsync(user, ct);
    }

    public async Task VerifyEmailAsync(string token, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(token, ct)
            ?? throw new InvalidTokenException("Token za potvrdu naloga nije ispravan.");

        user.VerifyEmail(token);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ResendVerificationAsync(string email, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);

        // Ne otkrivamo da li nalog postoji - ako ne postoji ili je vec
        // verifikovan, operacija se tiho zavrsava bez slanja poruke.
        if (user is null || user.IsEmailVerified)
            return;

        var token = Guid.NewGuid().ToString("N");
        user.RegenerateVerificationToken(token);

        var verifyUrl = $"{_clientOptions.Value.BaseUrl}/verify-email?token={token}";
        var (subject, body) = EmailTemplates.EmailVerification(user.FirstName, verifyUrl);
        await _outbox.AddAsync(
            OutboxMessage.Create(OutboxMessageType.EmailVerification, user.Email, subject, body, _clock.UtcNow), ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<TokenResponseDto> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken, ct)
            ?? throw new InvalidTokenException("Token za osvezavanje nije ispravan.");

        if (!existing.IsActive(_clock.UtcNow))
            throw new InvalidTokenException("Token za osvezavanje je istekao ili opozvan.");

        var user = await _userRepository.GetByIdAsync(existing.UserId, ct)
            ?? throw new InvalidTokenException("Korisnik vise ne postoji.");

        existing.Revoke(_clock.UtcNow);

        return await IssueTokensAsync(user, ct);
    }

    public async Task ForgotPasswordAsync(string email, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);

        if (user is null)
            return; // ne otkrivamo postojanje naloga

        var token = Guid.NewGuid().ToString("N");
        user.RequestPasswordReset(token, _clock.UtcNow.AddHours(1));

        var resetUrl = $"{_clientOptions.Value.BaseUrl}/reset-password?token={token}";
        var (subject, body) = EmailTemplates.PasswordReset(user.FirstName, resetUrl);
        await _outbox.AddAsync(
            OutboxMessage.Create(OutboxMessageType.PasswordReset, user.Email, subject, body, _clock.UtcNow), ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(string token, string newPassword, CancellationToken ct)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(token, ct)
            ?? throw new InvalidTokenException("Token za promenu lozinke nije ispravan.");

        user.ResetPassword(token, _passwordHasher.Hash(newPassword), _clock.UtcNow);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<TokenResponseDto> IssueTokensAsync(User user, CancellationToken ct)
    {
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshTokenValue();
        var refreshToken = RefreshToken.Create(
            user.Id, refreshTokenValue, _clock.UtcNow, _jwtOptions.Value.RefreshTokenDays);

        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new TokenResponseDto(
            accessToken,
            refreshTokenValue,
            _clock.UtcNow.AddMinutes(_jwtOptions.Value.AccessTokenMinutes),
            user.ToDto());
    }
}
