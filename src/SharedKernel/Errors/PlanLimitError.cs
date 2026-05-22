namespace MicroCrm.SharedKernel.Errors;

/// <summary>
/// Limite do plano atingido. Vira HTTP 402 (Payment Required).
/// Ex.: o tenant chegou no número máximo de clientes do plano atual.
/// </summary>
public sealed class PlanLimitError : AppError
{
    public PlanLimitError(string message, string code = "plan_limit")
        : base(code, message, httpStatus: 402)
    {
    }
}
