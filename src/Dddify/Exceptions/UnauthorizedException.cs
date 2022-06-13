namespace Dddify.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
       : base("The specified resource was not authorized.")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
