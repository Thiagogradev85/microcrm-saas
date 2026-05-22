namespace MicroCrm.SharedKernel.Abstractions;

/// <summary>
/// Fornece o usuário autenticado da requisição atual (lido do token JWT).
/// A implementação concreta vive na Infrastructure.
/// </summary>
public interface ICurrentUser
{
    /// <summary>Id do usuário logado.</summary>
    Guid UserId { get; }

    /// <summary>Tenant ao qual o usuário pertence.</summary>
    Guid TenantId { get; }

    /// <summary>E-mail do usuário (pode ser null se não autenticado).</summary>
    string? Email { get; }

    /// <summary>Papéis do usuário: SuperAdmin, TenantAdmin ou User.</summary>
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>True quando há um usuário autenticado na requisição.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Atalho para verificar se o usuário tem um papel específico.</summary>
    bool IsInRole(string role);
}
