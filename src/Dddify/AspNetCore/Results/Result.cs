namespace Dddify.Results;

public class Result<T>
{
    public bool IsSuccess { get; }

    public T? Value { get; }

    public string? Message { get; }

    private Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    private Result(bool isSuccess, T value)
    {
        IsSuccess = isSuccess;
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Success(string message) => new(true, message);
    public static Result<T> Failure(T value) => new(false, value);
    public static Result<T> Failure(string message) => new(false, message);

}
