using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.DTOs.Common
{
    public class BaseResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }

        public static BaseResult<T> Success(T value) => new BaseResult<T> { IsSuccess = true, Value = value };
        public static BaseResult<T> Failure(string error) => new BaseResult<T> { IsSuccess = false, Error = error };
    }
}
