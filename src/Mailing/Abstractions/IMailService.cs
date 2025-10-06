namespace Avolutions.Baf.Core.Mailing.Abstractions;

public interface IMailService
{
    Task  SendAsync(
        string to,
        string subject,
        string body,
        Dictionary<string, byte[]>? attachments = null,
        CancellationToken ct = default);

    Task<(bool Success, string Message)> TryConnectAsync(
        string host,
        int port,
        string user,
        string password,
        CancellationToken ct = default);
}