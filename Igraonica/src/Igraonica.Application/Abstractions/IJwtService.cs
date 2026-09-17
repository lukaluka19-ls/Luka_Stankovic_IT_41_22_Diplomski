using Igraonica.Domain.Entities;

namespace Igraonica.Application.Abstractions;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshTokenValue();
}
