namespace BlogApp.Domain.Common.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult(string message)
            : base(false, message)
        {
        }

        public ErrorResult(string message, List<string> errors)
            : base(false, message, errors)
        {
        }

        public ErrorResult()
            : base(false)
        {
        }
    }
}