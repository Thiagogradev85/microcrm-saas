namespace MicroCrm.SharedKernel.Abstractions;

/// <summary>
/// Fornece o tenant (empresa cliente) da requisição atual.
///
/// É a peça central do isolamento multitenant: os HasQueryFilter do EF Core usam
/// <see cref="CurrentTenantId"/> para que cada tenant só enxergue os próprios dados.
/// A implementação concreta vive na Infrastructure (lê o tenant do token/contexto).
/// </summary>
public interface ITenantContext
{
    /// <summary>Id do tenant corrente. Só faz sentido quando <see cref="HasTenant"/> é true.</summary>
    Guid CurrentTenantId { get; }

    /// <summary>
    /// Indica se há um tenant resolvido. Rotas públicas (ex.: login, cadastro de
    /// tenant) podem rodar sem tenant — por isso checamos antes de usar o Id.
    /// </summary>
    bool HasTenant { get; }
}
