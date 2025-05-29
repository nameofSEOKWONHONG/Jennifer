namespace Jennifer.SharedKernel;

public interface IDomainEventHandler<in T> where T : IDomainEvent;
