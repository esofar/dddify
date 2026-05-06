using Microsoft.Extensions.Localization;

namespace Dddify.Localization.Internal;

public class StringLocalizer(IStringLocalizerFactory factory) : IStringLocalizer
{
    private readonly IStringLocalizer _localizer = factory.Create(typeof(SharedResource));

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}