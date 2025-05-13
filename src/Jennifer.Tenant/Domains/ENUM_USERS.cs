using Ardalis.SmartEnum;

namespace Jennifer.Tenant.Domains;

public class ENUM_USER_TYPE : SmartEnum<ENUM_USER_TYPE, int>
{
    public static readonly ENUM_USER_TYPE ADMIN = new ENUM_USER_TYPE("ADMIN", 1);
    public static readonly ENUM_USER_TYPE CUSTOMER = new ENUM_USER_TYPE("CUSTOMER", 2);
    public static readonly ENUM_USER_TYPE DELEVER = new ENUM_USER_TYPE("DELEVER", 3);
    public ENUM_USER_TYPE(string name, int value) : base(name, value)
    {
        
    }
}