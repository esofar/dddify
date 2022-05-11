using Dddify.Guids;
using Dddify.Security.Identity;
using Dddify.Timing;
using MediatR;

namespace Dddify.EntityFrameworkCore;

public interface IDbContextDependencies
{
    ICurrentUser CurrentUser { get; }

    IClock Clock { get; }

    IPublisher Publisher { get; }

    IGuidGenerator GuidGenerator { get; }
}
