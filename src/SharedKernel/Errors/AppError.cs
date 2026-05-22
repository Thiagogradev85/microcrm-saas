namespace MicroCrm.SharedKernel.Errors;

/// <summary>
/// Representa um erro de domínio/aplicação de forma TIPADA.
///
/// Em vez de lançar uma Exception genérica, os métodos do projeto retornam um
/// AppError (normalmente embrulhado num <c>Result&lt;T&gt;</c>). Cada subtipo
/// concreto já carrega o status HTTP correto, então a camada de API traduz o
/// erro para a resposta certa sem precisar de uma cadeia gigante de 'if'.
/// </summary>
public abstract class AppError
{
    protected AppError(string code, string message, int httpStatus)
    {
        Code = code;
        Message = message;
        HttpStatus = httpStatus;
    }

    /// <summary>Código curto e estável do erro (ex.: "client.not_found"). Bom para o front tratar.</summary>
    public string Code { get; }

    /// <summary>Mensagem legível para humanos.</summary>
    public string Message { get; }

    /// <summary>Status HTTP que a API deve devolver para este erro.</summary>
    public int HttpStatus { get; }
}
