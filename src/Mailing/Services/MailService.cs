using Avolutions.Baf.Core.Mailing.Abstractions;
using Avolutions.Baf.Core.Mailing.Resources;
using Avolutions.Baf.Core.Mailing.Settings;
using Avolutions.Baf.Core.Settings.Abstractions;
using MailKit.Security;
using Microsoft.Extensions.Localization;

namespace Avolutions.Baf.Core.Mailing.Services;

public class MailService : IMailService
{
    private readonly ISettings<MailSettings> _settings;
    private readonly IStringLocalizer<MailingResources> _localizer;

    public MailService(
        ISettings<MailSettings> settings,
        IStringLocalizer<MailingResources> localizer)
    {
        _settings = settings;
        _localizer = localizer;
    }

    public void Send(string to, string subject, string body, params string[] attachments)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Success, string Message)> TryConnectAsync(string host, int port, string user, string password, CancellationToken ct = default)
    {
        using var client = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            client.Timeout = 10000;
            await client.ConnectAsync(host, port, SecureSocketOptions.Auto, ct);
            await client.AuthenticateAsync(user, password, ct);
            await client.DisconnectAsync(true, ct);
            
            return (true, _localizer["ConnectionSuccess"]);
        }
        catch (Exception ex)
        {
            try
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, ct);
                }
            }
            catch { /* ignore */ }

            return (false, _localizer["ConnectionError", ex.Message]);
        }
    }
}