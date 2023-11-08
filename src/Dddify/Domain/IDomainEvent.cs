using MediatR;

namespace Dddify.Domain;

/// <summary>
/// Represents a domain events.
/// </summary>
public interface IDomainEvent : INotification
{
}