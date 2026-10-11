namespace TalaanNiAna.Application.Common.Exceptions;

/// <summary>The request is malformed. Maps to 400 with per-field errors.</summary>
public sealed class RequestValidationException : DomainException
{
    public RequestValidationException(IDictionary<string, string[]> errors)
        : base("Hindi tama ang ilang detalye.")
    {
        Errors = errors;
    }

    public RequestValidationException(string field, string message)
        : this(new Dictionary<string, string[]> { [field] = [message] }) { }

    public IDictionary<string, string[]> Errors { get; }

    public override string Code => "validation_failed";
}
