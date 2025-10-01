namespace Avolutions.Baf.Core.Mailing.Abstractions;

public interface IMailService
{
    void Send(string to, string subject, string body, params string[] attachments);

    Task<(bool Success, string Message)> TryConnectAsync(
        string host,
        int port,
        string user,
        string password,
        CancellationToken ct = default);
}