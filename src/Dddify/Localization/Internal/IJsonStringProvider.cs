using System.Globalization;

namespace Dddify.Localization.Internal;

public interface IJsonStringProvider
{
    IList<string>? GetAllResourceStrings(CultureInfo culture, bool throwOnMissing);
}