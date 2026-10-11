namespace TalaanNiAna.Application.Common.Exceptions;

/// <summary>Base for errors the caller can act on. Mapped to a status code by the middleware.</summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }

    /// <summary>Short, stable code for the client to branch on.</summary>
    public abstract string Code { get; }
}
