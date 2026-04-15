namespace DnDPlatform.Services.Events;

public interface IEventBus
{
    Task PublishAsync<T>(T domainEvent) where T : DomainEvent;
    void Subscribe<T>(Func<T, Task> handler) where T : DomainEvent;
}
