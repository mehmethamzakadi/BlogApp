namespace BlogApp.Domain.Common.Results
{
    public class Result : IResult
    {
        public Result(bool success, string message)
            : this(success)
        {
            Message = message;
        }

        public Result(bool success, string message, List<string> errors)
            : this(success, message)
        {
            Errors = errors;
        }

        public Result(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
        public string Message { get; } = string.Empty;
        public List<string> Errors { get; } = new();
    }
}
