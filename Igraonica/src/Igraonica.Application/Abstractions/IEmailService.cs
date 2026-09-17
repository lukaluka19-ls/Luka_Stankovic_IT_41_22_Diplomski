namespace Igraonica.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(string toAddress, string subject, string htmlBody, CancellationToken ct);
}
