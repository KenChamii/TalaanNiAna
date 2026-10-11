namespace TalaanNiAna.Application.Common.Exceptions;

/// <summary>The requested record does not exist. Maps to 404.</summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entity, object key)
        : base($"{entity} {key} was not found.") { }

    public override string Code => "not_found";
}
