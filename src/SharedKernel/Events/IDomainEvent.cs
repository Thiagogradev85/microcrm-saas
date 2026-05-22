namespace MicroCrm.SharedKernel.Events;

/// <summary>
/// Contrato de um Evento de Domínio: algo relevante que JÁ aconteceu no domínio
/// (ex.: ClientCreated, DealWon). Por isso o nome de um evento é sempre no passado.
///
/// É a única forma de um módulo "avisar" o outro (via RabbitMQ). Chamada direta
/// entre módulos é proibida no projeto.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único do evento. Serve para garantir idempotência:
    /// se o mesmo evento chegar duas vezes, o handler usa esse Id para não
    /// aplicar o efeito em dobro (regra crítica nº 7).
    /// </summary>
    Guid EventId { get; }

    /// <summary>Momento (em UTC) em que o evento ocorreu. Nunca usar horário local.</summary>
    DateTime OccurredOnUtc { get; }
}
