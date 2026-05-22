using MicroCrm.SharedKernel.Events;

namespace MicroCrm.SharedKernel.Abstractions;

/// <summary>
/// Publica Domain Events para os outros módulos (implementação concreta usa o
/// RabbitMQ.Client). É o "canal oficial" de comunicação entre Bounded Contexts —
/// nenhum módulo chama o outro diretamente.
/// </summary>
public interface IEventBus
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
