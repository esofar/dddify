namespace Dddify.AspNetCore.Results;

public interface IApiResultWrapper
{
    IApiResult Succeed();

    IApiResult<T> Succeed<T>(T data);

    IApiResult Failed();

    IApiResult Failed(string errorMessage);

    IApiResult Failed(string errorCode, string errorMessage);

    IApiResultWithError Failed(IDictionary<string, string[]> error);
}