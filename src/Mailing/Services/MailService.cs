using Avolutions.Baf.Core.Mailing.Abstractions;
using Avolutions.Baf.Core.Mailing.Resources;
using Avolutions.Baf.Core.Mailing.Settings;
using Avolutions.Baf.Core.Settings.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Localization;
using MimeKit;

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

    public async Task SendAsync(string to, string subject, string body, Dictionary<string, byte[]>? attachments = null, CancellationToken ct = default)
    {
        var settings = _settings.Value;
        
        await settings.Password.UseAsync(async password =>
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(settings.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            if (attachments != null)
            {
                foreach (var (filename, bytes) in attachments)
                    builder.Attachments.Add(filename, bytes);
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                client.Timeout = 10000;

                await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.Auto, ct);
                await client.AuthenticateAsync(settings.User, password, ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);
            }
            catch
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, ct);
                }
                throw;
            }
        });
    }

    public async Task<(bool Success, string Message)> TryConnectAsync(string host, int port, string user, string password, CancellationToken ct = default)
    {
        using var client = new SmtpClient();
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