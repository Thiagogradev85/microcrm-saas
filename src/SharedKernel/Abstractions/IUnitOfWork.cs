namespace MicroCrm.SharedKernel.Abstractions;

/// <summary>
/// Unit of Work: confirma todas as alterações pendentes numa única transação.
///
/// Os repositórios apenas marcam o que mudou (Add/Update/Remove); só aqui as
/// mudanças vão de fato para o banco — ou todas juntas, ou nenhuma.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persiste as alterações pendentes. Retorna quantas linhas foram afetadas.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
