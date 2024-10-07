using Dddify.Guids;
using System.Collections.Concurrent;

namespace Dddify;

public partial class DddifyOptions
{
    private readonly ConcurrentDictionary<Type, IOptionsExtension> _extensionsMap;
    private readonly List<Type> _openBehaviors;

    public DddifyOptions()
    {
        _extensionsMap = new ConcurrentDictionary<Type, IOptionsExtension>();
        _openBehaviors = new List<Type>();
    }

    public List<Type> OpenBehaviors => _openBehaviors;

    public List<IOptionsExtension> Extensions => [.. _extensionsMap.Values];

    public DateTimeKind DateTimeKind { get; set; } = DateTimeKind.Unspecified;

    public SequentialGuidType SequentialGuidType { get; set; } = SequentialGuidType.SequentialAsString;

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