using System.Text.Json.Serialization;

namespace Dddify.AspNetCore.Results;

public class ApiResult : IApiResult
{
    [JsonPropertyOrder(0)]
    public bool Success { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }
}

public class ApiResult<T> : ApiResult, IApiResult<T>
{
    [JsonPropertyOrder(1)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }
}

public class ApiResultWithError : ApiResult, IApiResultWithError
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Error { get; set; }
}