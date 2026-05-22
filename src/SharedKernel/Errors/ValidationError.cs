namespace MicroCrm.SharedKernel.Errors;

/// <summary>Dados de entrada inválidos. Vira HTTP 422 (Unprocessable Entity).</summary>
public sealed class ValidationError : AppError
{
    public ValidationError(string message, string code = "validation")
        : base(code, message, httpStatus: 422)
    {
    }

    // Mais pra frente, quando integrarmos o FluentValidation, este erro pode
    // ganhar uma coleção de erros por campo (ex.: { "Email": ["é obrigatório"] }).
}
