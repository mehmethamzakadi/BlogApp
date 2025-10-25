namespace BlogApp.Domain.Common.Results
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
        List<string> Errors { get; }
    }
}
