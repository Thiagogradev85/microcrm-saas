namespace MicroCrm.SharedKernel.Primitives;

/// <summary>
/// Classe base de um Value Object: um objeto SEM identidade própria, comparado
/// por VALOR. Ex.: dois "Endereço" são iguais se rua, número e cidade são iguais.
///
/// São imutáveis (sem setters públicos). Cada VO concreto só precisa dizer QUAIS
/// campos entram na comparação, implementando <see cref="GetEqualityComponents"/>.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Devolve, em ordem, os componentes que definem a igualdade do VO.
    /// Ex.: para um Endereço, seria: yield return Rua; yield return Numero; ...
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;

        var other = (ValueObject)obj;

        // SequenceEqual (LINQ): compara as duas listas item a item, na mesma ordem.
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        // HashCode.Add combina os componentes num único hash de forma segura.
        var hash = new HashCode();

        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
