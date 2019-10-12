using System;
using DotNetCore30Demo.Resource.Enum;
using DotNetCore30Demo.Resource.Response;

namespace DotNetCore30Demo.Model
{
    public class CustomExceptionResponse:BaseResultResponse
    {
        public CustomExceptionResponse(int? code, Exception exception)
        {
            Code = code;
            Message = exception.InnerException != null ?
                exception.InnerException.Message :
                exception.Message;
            Result = exception.Message;
            ReturnStatus = StatusResponseEnum.Error;
        }
    }
}