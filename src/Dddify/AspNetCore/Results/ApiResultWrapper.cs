using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dddify.AspNetCore.Results;

public class ApiResultWrapper : IApiResultWrapper
{
    private readonly ApiResultWrapperOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiResultWrapper(IOptions<ApiResultWrapperOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? TraceId => _options.EnableTraceIdentifier ? _httpContextAccessor.HttpContext?.TraceIdentifier : null;

    public IApiResult Succeed() => new ApiResult
    {
        Success = true,
        TraceId = TraceId,
    };

    public IApiResult<T> Succeed<T>(T data) => new ApiResult<T>
    {
        Success = true,
        TraceId = TraceId,
        Data = data,
    };

    public IApiResult Failed() => new ApiResult
    {
        Success = false,
        TraceId = TraceId,
    };

    public IApiResult Failed(string errorMessage) => new ApiResult
    {
        Success = false,
        TraceId = TraceId,
        ErrorMessage = errorMessage,
    };

    public IApiResult Failed(string errorCode, string errorMessage) => new ApiResult
    {
        Success = false,
        TraceId = TraceId,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage,
    };

    public IApiResultWithError Failed(IDictionary<string, string[]> error) => new ApiResultWithError
    {
        Success = false,
        TraceId = TraceId,
        Error = error,
    };
}