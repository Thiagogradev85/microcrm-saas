using MicroCrm.SharedKernel.Events;

namespace MicroCrm.SharedKernel.Primitives;

/// <summary>
/// Raiz de um Agregado. É o único ponto de entrada para alterar o agregado e
/// o único tipo que os repositórios carregam e salvam.
///
/// Além de ser uma <see cref="Entity"/>, acumula os Domain Events ocorridos
/// durante a operação. Depois de salvar, a infraestrutura lê esses eventos e os
/// publica no RabbitMQ — é assim que os módulos se comunicam (sem chamada direta).
/// </summary>
public abstract class AggregateRoot : Entity
{
    // 'readonly': a lista nunca é trocada por outra; só adicionamos/limpamos itens.
    private readonly List<IDomainEvent> _domainEvents = new();

    protected AggregateRoot(Guid id) : base(id) { }

    protected AggregateRoot() { }

    /// <summary>
    /// Eventos pendentes de publicação. Exposto como somente-leitura para que
    /// ninguém de fora consiga adicionar/remover direto na lista interna.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // 'protected': só o próprio agregado registra seus eventos, de dentro dos seus
    // métodos de negócio. (Em alguns projetos esse método se chama RaiseDomainEvent.)
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Limpa os eventos já publicados. Chamado pela infraestrutura após o publish.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
