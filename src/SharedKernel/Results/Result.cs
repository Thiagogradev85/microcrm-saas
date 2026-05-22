using MicroCrm.SharedKernel.Errors;

namespace MicroCrm.SharedKernel.Results;

/// <summary>
/// Representa o resultado de uma operação que pode dar certo OU falhar, SEM usar
/// exceções para controle de fluxo. É o padrão de retorno principal do projeto.
///
/// Use esta versão (não genérica) quando a operação não devolve um valor — só
/// precisa dizer "deu certo" ou "deu erro" (ex.: um Delete).
/// </summary>
public class Result
{
    protected Result(bool isSuccess, AppError? error)
    {
        // Invariantes: sucesso nunca carrega erro; falha sempre carrega um erro.
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Um resultado de sucesso não pode conter erro.");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Um resultado de falha precisa conter um erro.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    /// <summary>O erro, quando <see cref="IsFailure"/> for true. Null em caso de sucesso.</summary>
    public AppError? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(AppError error) => new(false, error);
}

/// <summary>
/// Versão que carrega um VALOR quando dá certo (ex.: o Client criado).
/// </summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(bool isSuccess, T? value, AppError? error)
        : base(isSuccess, error)
        => _value = value;

    /// <summary>
    /// O valor produzido pela operação. Só pode ser lido em caso de sucesso —
    /// sempre cheque IsSuccess antes, senão estoura InvalidOperationException.
    /// </summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível ler Value de um resultado de falha.");

    public static Result<T> Success(T value) => new(true, value, null);

    // 'new' = esconde o Failure da classe base, porque aqui queremos devolver
    // um Result<T> (e não um Result simples).
    public static new Result<T> Failure(AppError error) => new(false, default, error);

    // Conversões implícitas (açúcar sintático que vale a pena):
    //   return client;                 -> vira Result<Client>.Success(client)
    //   return new NotFoundError(...);  -> vira Result<Client>.Failure(erro)
    // Assim os handlers ficam limpos, sem ".Success(...)" / ".Failure(...)" o tempo todo.
    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(AppError error) => Failure(error);
}
