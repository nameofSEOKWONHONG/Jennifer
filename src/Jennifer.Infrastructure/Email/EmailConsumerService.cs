using Confluent.Kafka;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.MessageQueues;
using Jennifer.SharedKernel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Jennifer.Infrastructure.Email;

public class EmailConsumerService(ILogger<EmailConsumerService> logger, 
    IConsumer<string, string> consumer, 
    IJMongoFactory factory,
    IServiceScopeFactory serviceScopeFactory) : KafkaConsumerServiceBase<EmailConsumerService, JenniferDbContext>(logger, consumer, serviceScopeFactory, "email")
{
    protected override async Task ConsumeAsync(IServiceProvider service, ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        if(consumeResult.xIsEmpty()) return;
        
        var message = consumeResult.Message.Value;
        var emailMessage = message.xDeserialize<EmailMessage>();
        await SendEmailAsync(emailMessage);
    }
    
    private async Task SendEmailAsync(EmailMessage email)
    {
        var document = new EmailSendResultDocument();
            
        try
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

            document.ToEmail = email.To.Select(m => m.To).ToList().xJoin();
            document.Src = (new {email.From, email.To, email.Cc, email.IsHtml}).xSerialize();

            using var client = new SmtpClient();
            await client.ConnectAsync(JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpHost,
                JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpUser,
                JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            document.Result = "success";
        }
        catch (Exception ex)
        {
            document.Result = ex.Message;
            throw;
        }
        
        document.CreatedAt = DateTimeOffset.UtcNow;
        var collection = factory.Create<EmailSendResultDocument>().GetCollection();
        await collection.InsertOneAsync(document);
    }
}