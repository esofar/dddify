namespace Dddify.AspNetCore.ApiResult;

public class ApiResult : IApiResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string TraceId { get; set; }
}

public class ApiResult<T> : ApiResult, IApiResult<T>
{
    public T Data { get; set; }
}

public class ApiResultWithError : ApiResult, IApiResultWithError
{
    public IDictionary<string, string[]> Error { get; set; }
}
