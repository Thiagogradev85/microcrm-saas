namespace MicroCrm.SharedKernel.Errors;

/// <summary>
/// Erro genérico de regra de domínio que não se encaixa nos outros tipos.
/// Vira HTTP 400 (Bad Request).
/// </summary>
public sealed class DomainError : AppError
{
    public DomainError(string message, string code = "domain")
        : base(code, message, httpStatus: 400)
    {
    }
}
