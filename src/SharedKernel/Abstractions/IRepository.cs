using MicroCrm.SharedKernel.Primitives;

namespace MicroCrm.SharedKernel.Abstractions;

/// <summary>
/// Contrato genérico de repositório.
///
/// 'where T : AggregateRoot' restringe o uso: em DDD, só carregamos e salvamos
/// agregados pela sua raiz — nunca entidades internas soltas.
///
/// Repare que NÃO há método de "salvar" aqui: persistir de fato é responsabilidade
/// do <see cref="IUnitOfWork"/>, que confirma tudo numa transação só.
/// </summary>
public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Update e Remove são síncronos: com o EF Core, eles só marcam a entidade
    // (tracking); a ida ao banco acontece no SaveChangesAsync do UnitOfWork.
    void Update(T entity);

    void Remove(T entity);
}
