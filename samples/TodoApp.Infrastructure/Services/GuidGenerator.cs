using TodoApp.Application.Services;

namespace TodoApp.Infrastructure.Services;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class GuidGenerator : IGuidGenerator
{
    public Guid Create() => Guid.CreateVersion7();
}