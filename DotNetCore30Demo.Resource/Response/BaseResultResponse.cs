using DotNetCore30Demo.Resource.Enum;

namespace DotNetCore30Demo.Resource.Response
{
    public class BaseResultResponse
    {
        public BaseResultResponse(int? code = null, string message = null, object result = null, StatusResponseEnum returnStatus = StatusResponseEnum.Success)
        {
            Code = code;
            Result = result;
            Message = message;
            ReturnStatus = returnStatus;
        }
        public int? Code { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }

        public StatusResponseEnum ReturnStatus { get; set; }
    }
}