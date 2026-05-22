namespace MicroCrm.SharedKernel.Errors;

/// <summary>
/// Usuário autenticado, mas SEM permissão para a ação. Vira HTTP 403.
/// (403 Forbidden = "sei quem você é, mas você não pode fazer isso";
///  diferente de 401 Unauthorized, que é "não sei quem você é".)
/// </summary>
public sealed class UnauthorizedError : AppError
{
    public UnauthorizedError(string message, string code = "unauthorized")
        : base(code, message, httpStatus: 403)
    {
    }
}
