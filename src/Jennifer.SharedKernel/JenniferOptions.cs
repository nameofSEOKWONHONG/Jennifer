namespace Jennifer.SharedKernel;

public class JwtOptions
{
    public JwtOptions(string key, string issuer, string audience, int expireMinutes, int refreshExpireMinutes)
    {
        if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        if(string.IsNullOrWhiteSpace(issuer)) throw new ArgumentNullException(nameof(issuer));
        if(string.IsNullOrWhiteSpace(audience)) throw new ArgumentNullException(nameof(audience));
        if(expireMinutes <= 0) throw new ArgumentNullException(nameof(expireMinutes));
        if(refreshExpireMinutes <= 0) throw new ArgumentNullException(nameof(refreshExpireMinutes));
        
        Key = key;
        Issuer = issuer;
        Audience = audience;
        ExpireMinutes = expireMinutes;
        RefreshExpireMinutes = refreshExpireMinutes;
    }

    public string Key { get;  }
    public string Issuer { get;  }
    public string Audience { get;  }
    public int ExpireMinutes { get;  }
    public int RefreshExpireMinutes { get;  }
}

public class CryptoOptions
{
    public CryptoOptions(string aesKey, string aesIv)
    {
        if(string.IsNullOrWhiteSpace(aesKey)) throw new ArgumentNullException(nameof(aesKey));
        if(string.IsNullOrWhiteSpace(aesIv)) throw new ArgumentNullException(nameof(aesIv));
        
        AesKey = aesKey;
        AesIV = aesIv;
    }

    public string AesKey { get; }
    public string AesIV { get; }

}

public class EmailSmtpOptions
{
    public string SmtpHost { get; }
    public int SmtpPort { get; }
    public string SmtpUser { get; }
    public string SmtpPass { get; }

    public EmailSmtpOptions(string smtpHost, int smtpPort, string smtpUser, string smtpPass)
    {
        SmtpHost = smtpHost;
        SmtpPort = smtpPort;
        SmtpUser = smtpUser;
        SmtpPass = smtpPass;
    }
}

public class JenniferOptions
{
    public string Schema { get;  }
    public JwtOptions Jwt { get;  }
    public CryptoOptions Crypto { get;  }
    
    public EmailSmtpOptions EmailSmtp { get;  }

    public JenniferOptions(string schema,
        CryptoOptions cryptoOptions, 
        JwtOptions jwtOptions,
        EmailSmtpOptions emailSmtpOptions)
    {
        ArgumentNullException.ThrowIfNull(schema);
        
        ArgumentNullException.ThrowIfNull(cryptoOptions);
        ArgumentNullException.ThrowIfNull(jwtOptions);
        ArgumentNullException.ThrowIfNull(emailSmtpOptions);
        
        this.Schema = schema;
        
        this.Jwt = jwtOptions;
        this.Crypto = cryptoOptions;
        this.EmailSmtp = emailSmtpOptions;
    }
}

public class JenniferOptionSingleton
{
    private static readonly Lazy<JenniferOptionSingleton> _instance = new(() => new JenniferOptionSingleton());
    private static readonly object _lock = new();

    public static JenniferOptionSingleton Instance => _instance.Value;

    public JenniferOptions Options { get; private set; }

    public static void Attach(JenniferOptions options)
    {
        lock (_lock)
        {
            if (Instance.Options is not null)
                throw new InvalidOperationException("JenniferOptions already attached.");
            
            Instance.Options = options;
        }
    }
}