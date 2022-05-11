using Dddify.DependencyInjection;
using Dddify.Guids;
using Dddify.Security.Identity;
using Dddify.Timing;
using MediatR;

namespace Dddify.EntityFrameworkCore;

public class DbContextDependencies : IDbContextDependencies, ITransientDependency
{
    public DbContextDependencies(IClock clock, IPublisher publisher, ICurrentUser currentUser, IGuidGenerator guidGenerator)
    {
        Clock = clock;
        Publisher = publisher;
        CurrentUser = currentUser;
        GuidGenerator = guidGenerator;
    }

    public IClock Clock { get; }

    public IPublisher Publisher { get; }

    public ICurrentUser CurrentUser { get; }

    public IGuidGenerator GuidGenerator { get; }
}
