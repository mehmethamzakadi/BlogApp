namespace BlogApp.Domain.Common.Results
{
    public class Result<T>
    {
        // İşlem sonucunda dönecek veri
        public T Data { get; set; }

        // İşlemin başarılı olup olmadığını belirten alan
        public bool Success { get; set; }

        // İşlemle ilgili mesaj
        public string Message { get; set; }

        // Başarılı işlem sonucu döndürmek için statik bir metod
        public static Result<T> SuccessResult(T data, string message = "")
        {
            return new Result<T>
            {
                Data = data,
                Success = true,
                Message = message
            };
        }

        public static Result<T> SuccessResult(string message)
        {
            return new Result<T>
            {
                Data = default(T),
                Success = true,
                Message = message
            };
        }

        // Başarısız işlem sonucu döndürmek için statik bir metod
        public static Result<T> FailureResult(string message)
        {
            return new Result<T>
            {
                Data = default(T),
                Success = false,
                Message = message
            };
        }
    }

}