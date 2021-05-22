using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosFunction.Response
{
    public class Response<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public static Response<T> Success(T data)
        {
            return new Response<T> { Succeeded = true, Message = "Success", Data = data };
        }
        public static Response<T> Fail(string ErrorMessage)
        {
            return new Response<T> { Succeeded = false, Message = ErrorMessage };
        }

    }
}
