using System.Data;

namespace Jennifer.Infrastructure.Database;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class UseTransactionAttribute : Attribute
{
    public IsolationLevel IsolationLevel { get; }

    public UseTransactionAttribute(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        IsolationLevel = isolationLevel;
    }
}