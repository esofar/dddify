namespace Dddify.Exceptions;

/// <summary>
/// Represents domain layer exceptions, typically caused by violations of domain rules or invariants.
/// </summary>
/// <remarks>
/// This exception indicates a business rule violation within the core domain model.
/// It should be thrown from entities, value objects, or domain services when an operation would result in an invalid domain state.
/// </remarks>
public abstract class DomainException : BusinessException
{
    public override string Category => "Domain";
}
