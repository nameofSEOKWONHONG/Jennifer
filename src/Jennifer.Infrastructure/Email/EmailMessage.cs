using eXtensionSharp;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.StaticFiles;

namespace Jennifer.Infrastructure.Email;

/// <summary>
/// Represents an email message for use in email sending systems.
/// Provides properties to define the sender, recipients, subject,
/// body content, and email configuration, such as HTML formatting
/// and attachments. This class is designed for flexible email construction
/// through the use of the Builder design pattern.
/// </summary>
public class EmailMessage
{
    public string From { get; set; }
    public string FromName { get; set; }
    public List<TO> To { get; set; } = new();
    public List<CC> Cc { get; set; } = new();
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHtml { get; set; }
    public int RetryCount { get; set; }
    
    public List<EmailAttachment> Attachments { get; set; } = new();
    
    public class TO
    {
        public string To { get; set; }
        public string ToName { get; set; }

        public TO(string to, string toName)
        {
            To = to;
            ToName = toName;
        }
    }
    
    public class CC
    {
        public string Cc { get; set; }
        public string CcName { get; set; }

        public CC(string cc, string ccName)
        {
            Cc = cc;
            CcName = ccName;
        }
    }
}

public class EmailMessageBuilder
{
    private readonly EmailMessage _email = new();

    public EmailMessageBuilder From(string name, string from)
    {
        _email.FromName = name;
        _email.From = from;
        return this;
    }

    public EmailMessageBuilder To(string name, string to)
    {
        _email.To.Add(new (to, name));
        return this;
    }

    public EmailMessageBuilder Cc(string name, string cc)
    {
        _email.Cc.Add(new (cc, name));
        return this;
    }

    public EmailMessageBuilder Subject(string subject)
    {
        _email.Subject = subject;
        return this;
    }

    public EmailMessageBuilder Body(string body)
    {
        _email.Body = body;
        return this;
    }

    public EmailMessageBuilder IsHtml(bool isHtml)
    {
        _email.IsHtml = isHtml;
        return this;        
    }

    private static readonly FileExtensionContentTypeProvider _provider = new();
    public EmailMessageBuilder Attachments(string tempFullFileName, string srcFilename)
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

    public EmailMessage Build()
    {
        this._email.FromName = this._email.FromName.xValue<string>(this._email.From);
        if (this._email.FromName.xIsEmpty())
        {
            this._email.FromName = "Jennifer";
            this._email.From = JenniferOptionSingleton.Instance.Options.EmailSmtp.SmtpUser;
        }

        return _email;
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
