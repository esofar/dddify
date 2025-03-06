using Dddify.Guids;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Collections.Concurrent;

namespace Dddify;

public partial class DddifyOptions
{
    private readonly ConcurrentDictionary<Type, IOptionsExtension> _extensionsMap;

    public DddifyOptions()
    {
        _extensionsMap = new ConcurrentDictionary<Type, IOptionsExtension>();
    }

    public Action<MediatRServiceConfiguration>? ConfigureMediatr { get; set; }

    public Action<IImplementationTypeSelector>? ConfigureScrutor { get; set; }

    public List<IOptionsExtension> Extensions => [.. _extensionsMap.Values];

    public DateTimeKind DateTimeKind { get; set; } = DateTimeKind.Unspecified;

    public SequentialGuidType SequentialGuidType { get; set; } = SequentialGuidType.SequentialAsString;

    public bool ValidationBehaviourEnabled { get; set; } = true;

    public bool UnitOfWorkBehaviorEnabled { get; set; } = true;

    public void AddOrUpdateExtension<TExtension>(TExtension extension)
        where TExtension : IOptionsExtension
    {
        ArgumentNullException.ThrowIfNull(extension);

        _extensionsMap.AddOrUpdate(
            typeof(TExtension),
            extension,
            (key, oldValue) => oldValue = extension);
    }
}