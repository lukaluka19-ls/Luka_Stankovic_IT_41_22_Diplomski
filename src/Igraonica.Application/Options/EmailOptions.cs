namespace Igraonica.Application.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 1025;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = false;
    public string FromAddress { get; set; } = "igraonicajoyland@gmail.com";
    public string FromName { get; set; } = "JoyLand Igraonica";
}
