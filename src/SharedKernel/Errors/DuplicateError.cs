namespace MicroCrm.SharedKernel.Errors;

/// <summary>
/// Violação de unicidade. Vira HTTP 409 (Conflict).
/// Ex.: tentar atribuir uma UF que já tem vendedor naquele tenant.
/// </summary>
public sealed class DuplicateError : AppError
{
    public DuplicateError(string message, string code = "duplicate")
        : base(code, message, httpStatus: 409)
    {
    }
}
