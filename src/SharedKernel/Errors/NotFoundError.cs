namespace MicroCrm.SharedKernel.Errors;

/// <summary>Entidade não encontrada. Vira HTTP 404.</summary>
// 'sealed': não há intenção de herdar deste erro concreto. Deixa o código mais
// claro e ajuda o compilador a otimizar.
public sealed class NotFoundError : AppError
{
    public NotFoundError(string message, string code = "not_found")
        : base(code, message, httpStatus: 404)
    {
    }
}
