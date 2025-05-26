using eXtensionSharp;
using Jennifer.Infrastructure.Options;
using Jennifer.SharedKernel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Jennifer.Infrastructure.Email;

/// <summary>
/// A background service responsible for sending emails from a queue.
/// </summary>
public class EmailSenderService : BackgroundService
{
    private readonly IEmailQueue _emailQueue;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(IEmailQueue emailQueue, ILogger<EmailSenderService> logger)
    {
        _emailQueue = emailQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //TODO: Modify email sending logic here. use kafaka or rabbitmq
        while (!stoppingToken.IsCancellationRequested)
        {
            var email = await _emailQueue.DequeueAsync(stoppingToken);
            try
            {
                await SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "이메일 전송 중 오류 발생: {To}", email.To);

                if (email.RetryCount < 3)
                {
                    email.RetryCount++;
                    await Task.Delay(1000, stoppingToken);
                    await _emailQueue.EnqueueAsync(email, stoppingToken);
                }
                else
                {
                    _logger.LogWarning("이메일 전송 재시도 초과: {To}", email.To);
                    // 여기에 저장 로직 (DB or 로그 파일 등) 추가 가능
                }
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    private async Task SendEmailAsync(EmailMessage email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            email.FromName, email.From
        ));
        
        if(email.To.xIsEmpty()) throw new ArgumentNullException($"email.To is empty");
        foreach (var item in email.To)
        {
            message.To.Add(new MailboxAddress(item.ToName, item.To));            
        }

        if (email.Cc.xIsNotEmpty())
        {
            foreach (var item in email.Cc)
            {
                message.Cc.Add(new MailboxAddress(item.CcName, item.Cc));    
            }
        }
        
        message.Subject = email.Subject;
        var bodyBuilder = new BodyBuilder();
        if (email.IsHtml)
        {
            bodyBuilder.HtmlBody = email.Body;
        }
        else
        {
            bodyBuilder.TextBody = email.Body;
        }

        foreach (var attachment in email.Attachments)
        {
            await bodyBuilder.Attachments.AddAsync(
                attachment.FileName,
                new MemoryStream(attachment.File),
                ContentType.Parse(attachment.ContentType) // 예: "application/pdf"
            );
        }
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpHost,
            JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpPort, SecureSocketOptions.SslOnConnect);
        await client.AuthenticateAsync(JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpUser,
            JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}