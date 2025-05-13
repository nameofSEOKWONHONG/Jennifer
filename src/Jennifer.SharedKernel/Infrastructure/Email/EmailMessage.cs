using System.Net.Mail;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Jennifer.SharedKernel.Infrastructure.Email;

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

        public Builder Attachments(string tempFullFileName, string srcFilename)
        {
            var provider = new FileExtensionContentTypeProvider();
            var filename = Path.GetFileName(srcFilename);
            provider.TryGetContentType(filename, out var contentType);
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

public class EmailAttachment
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] File { get; set; } = default!;
}
