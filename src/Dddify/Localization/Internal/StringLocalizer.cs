using Microsoft.Extensions.Localization;

namespace Dddify.Localization.Internal;

public class StringLocalizer : IStringLocalizer
{
    private readonly IStringLocalizer _localizer;

    public StringLocalizer(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create(typeof(SharedResource));
    }

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}