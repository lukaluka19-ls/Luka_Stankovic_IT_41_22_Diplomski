using Igraonica.Application.Abstractions;
using Igraonica.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Igraonica.Infrastructure.Services;

/// <summary>
/// Salje poruku preko SMTP protokola. U razvojnom okruženju Host pokazuje na
/// Mailpit (bez autentikacije), a u produkciji na stvarni SMTP server naloga
/// igraonicajoyland@gmail.com, cije se korisnicko ime i lozinka ne nalaze u
/// izvornom kodu (NFZ-04), vec se ucitavaju iz lokalnog skladista tajni ili
/// promenljivih okruzenja.
/// </summary>
public class SmtpEmailService(IOptions<EmailOptions> _options, ILogger<SmtpEmailService> _logger) : IEmailService
{
    public async Task SendAsync(string toAddress, string subject, string htmlBody, CancellationToken ct)
    {
        var options = _options.Value;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        var socketOptions = options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
        await client.ConnectAsync(options.Host, options.Port, socketOptions, ct);

        if (!string.IsNullOrWhiteSpace(options.Username))
            await client.AuthenticateAsync(options.Username, options.Password, ct);

        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        _logger.LogInformation("Poslata poruka elektronske poste na {Recipient}: {Subject}", toAddress, subject);
    }
}
