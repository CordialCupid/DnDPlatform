using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DnDPlatform.Services.Events;

public class InProcessEventBus(ILogger<InProcessEventBus> logger) : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<DomainEvent, Task>>> _handlers = new();

    public void Subscribe<T>(Func<T, Task> handler) where T : DomainEvent
    {
        var key = typeof(T);
        _handlers.GetOrAdd(key, _ => []).Add(e => handler((T)e));
    }

    public async Task PublishAsync<T>(T domainEvent) where T : DomainEvent
    {
        var key = typeof(T);
        if (!_handlers.TryGetValue(key, out var handlers))
        {
            return;     
        }

        foreach (var handler in handlers)
        {
            try
            {
                await handler(domainEvent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Event handler failed for {EventType}", key.Name);
            }
        }
    }
}
