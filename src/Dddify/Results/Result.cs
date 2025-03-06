namespace Dddify.Results;

public class Result(bool isSuccess, string? message)
{
    public bool IsSuccess { get; } = isSuccess;
    public string? Message { get; } = message;

    public static Result Success() => new(true, default);
    public static Result Failure(string message) => new(false, message);
}

public class Result<TValue>(TValue? value, bool isSuccess, string? message) : Result(isSuccess, message)
{
    public TValue? Value { get; } = value;

    public static Result<TValue> Success(TValue value) => new(value, true, default);
    public static Result<TValue> Failure(TValue value) => new(value, false, default);
}

public static class ResultExtensions
{
    public static TResult Match<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue?, TResult> onSuccess,
        Func<string?, TResult> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Message);
    }
}
