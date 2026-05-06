using MediatR;

namespace Dddify.SharedKernel;

/// <summary>
/// Represents a domain event raised by an aggregate root.
/// </summary>
/// <remarks>
/// Domain events capture business-significant facts that occurred inside the domain model and can be
/// handled within the same bounded context or published to other parts of the system.
/// </remarks>
public interface IDomainEvent : INotification
{
}
