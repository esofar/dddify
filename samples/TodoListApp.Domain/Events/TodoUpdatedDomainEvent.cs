using Dddify.Domain;

namespace TodoListApp.Domain.Events;

public record TodoUpdatedDomainEvent(Guid TodoId, bool IsDone) : IDomainEvent;