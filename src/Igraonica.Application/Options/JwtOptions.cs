namespace Igraonica.Application.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "IgraonicaAPI";
    public string Audience { get; set; } = "IgraonicaWeb";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
