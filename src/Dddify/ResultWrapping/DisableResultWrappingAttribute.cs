namespace Dddify.ResultWrapping;

/// <summary>
/// Disables API result wrapping for the decorated controller or action.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableResultWrappingAttribute : Attribute
{
}