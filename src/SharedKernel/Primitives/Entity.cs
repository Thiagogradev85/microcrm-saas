namespace MicroCrm.SharedKernel.Primitives;

/// <summary>
/// Classe base de toda entidade de domínio.
///
/// Uma entidade tem IDENTIDADE própria (o Id): dois objetos são "o mesmo"
/// quando têm o mesmo Id, mesmo que os demais campos sejam diferentes.
/// Isso é o oposto de um <see cref="ValueObject"/>, que compara por valor.
/// </summary>
public abstract class Entity
{
    /// <summary>Identificador único da entidade.</summary>
    // 'init' = só pode ser atribuído na construção do objeto; depois vira imutável.
    public Guid Id { get; protected init; }

    protected Entity(Guid id) => Id = id;

    // Construtor sem parâmetros exigido pelo EF Core para reconstruir a entidade
    // vinda do banco. 'protected' impede que seja usado fora da hierarquia.
    protected Entity() { }

    public override bool Equals(object? obj)
    {
        // Tem que ser uma Entity, do MESMO tipo concreto, e ter o mesmo Id.
        if (obj is not Entity other) return false;
        if (GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    // Sempre que sobrescrevemos Equals, o C# nos obriga a sobrescrever GetHashCode
    // junto, para manter a coerência: objetos iguais precisam ter o mesmo hash.
    public override int GetHashCode() => Id.GetHashCode();

    // Operadores '==' e '!=' para conseguirmos comparar entidades diretamente.
    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
