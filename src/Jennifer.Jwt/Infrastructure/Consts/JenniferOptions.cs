using eXtensionSharp;

namespace Jennifer.Jwt.Infrastructure.Consts;

public class JwtOptions
{
    public JwtOptions(string key, string issuer, string audience, int expireMinutes, int refreshExpireMinutes)
    {
        if(key.xIsEmpty()) throw new ArgumentNullException(nameof(key));
        if(issuer.xIsEmpty()) throw new ArgumentNullException(nameof(issuer));
        if(audience.xIsEmpty()) throw new ArgumentNullException(nameof(audience));
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
        if(aesKey.xIsEmpty()) throw new ArgumentNullException(nameof(aesKey));
        if(aesIv.xIsEmpty()) throw new ArgumentNullException(nameof(aesIv));
        
        AesKey = aesKey;
        AesIV = aesIv;
    }

    public string AesKey { get; }
    public string AesIV { get; }

}

public class JenniferOptions
{
    public string Schema { get;  }
    public JwtOptions Jwt { get;  }
    public CryptoOptions Crypto { get;  }

    public JenniferOptions(string schema, CryptoOptions cryptoOptions, JwtOptions jwtOptions)
    {
        if(schema.xIsEmpty()) throw new ArgumentNullException(nameof(schema));
        if(jwtOptions.xIsEmpty()) throw new ArgumentNullException(nameof(JwtOptions));
        if(cryptoOptions.xIsEmpty()) throw new ArgumentNullException(nameof(CryptoOptions));
        
        this.Schema = schema;
        this.Jwt = jwtOptions;
        this.Crypto = cryptoOptions;
    }
}

internal class JenniferOptionSingleton
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