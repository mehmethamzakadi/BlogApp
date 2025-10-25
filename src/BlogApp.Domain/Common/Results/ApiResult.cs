namespace BlogApp.Domain.Common.Results
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string InternalMessage { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class ApiReturn : ApiResult<object>
    {
    }
}
