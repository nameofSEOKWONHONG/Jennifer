using System.Net.Mail;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Jennifer.SharedKernel.Infrastructure.Email;

/// <summary>
/// Represents an email message for use in email sending systems.
/// Provides properties to define the sender, recipients, subject,
/// body content, and email configuration, such as HTML formatting
/// and attachments. This class is designed for flexible email construction
/// through the use of the Builder design pattern.
/// </summary>
public class EmailMessage
{
    public string From { get; private set; }
    public string FromName { get; private set; }
    public List<string> To { get; private set; } = new();
    public List<string> ToName { get; private set; } = new();
    public List<string> Cc { get; private set; } = new();
    public List<string> CcName { get; private set; }  = new();
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public bool IsHtml { get; private set; }
    public int RetryCount { get; set; }
    
    public List<EmailAttachment> Attachments { get; private set; } = new();

    private EmailMessage() { }

    public class Builder
    {
        private readonly EmailMessage _email = new();

        public Builder From(string name, string from)
        {
            _email.From = from;
            _email.FromName = name;
            return this;
        }

        public Builder To(string name, string to)
        {
            _email.ToName.Add(name);
            _email.To.Add(to);
            return this;
        }

        public Builder Cc(string name, string cc)
        {
            _email.CcName.Add(name);
            _email.Cc.Add(cc);
            return this;
        }

        public Builder Subject(string subject)
        {
            _email.Subject = subject;
            return this;
        }

        public Builder Body(string body, bool isHtml = true)
        {
            _email.Body = body;
            _email.IsHtml = isHtml;
            return this;
        }

        private static readonly FileExtensionContentTypeProvider _provider = new();
        public Builder Attachments(string tempFullFileName, string srcFilename)
        {
            var filename = Path.GetFileName(srcFilename);
            _provider.TryGetContentType(filename, out var contentType);
            
            if (!File.Exists(tempFullFileName))
                throw new FileNotFoundException("Attachment file not found.", tempFullFileName);

            _email.Attachments.Add(new EmailAttachment()
            {
                FileName = filename,
                ContentType = contentType ?? "application/octet-stream",
                File = File.ReadAllBytes(tempFullFileName)
            });
            return this;
        }

        public EmailMessage Build() => _email;
    }
}

/// <summary>
/// Represents a single attachment in an email message. This class provides
/// the file name, content type, and file content as a byte array, enabling
/// the inclusion of attachments in email messages.
/// </summary>
public class EmailAttachment
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] File { get; set; } = default!;
}
