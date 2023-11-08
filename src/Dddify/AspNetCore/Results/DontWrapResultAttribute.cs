namespace Dddify.AspNetCore.Results;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DontWrapResultAttribute : Attribute
{
}