using DotNetCore30Demo.Resource.Enum;
using DotNetCore30Demo.Resource.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace DotNetCore30Demo.Model
{
    public class ValidationFailedResultResponse:BaseResultResponse
    {
        public ValidationFailedResultResponse(ModelStateDictionary modelState)
        {
            Code = 422;
            Message = "参数不合法";
            Result = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
            ReturnStatus = StatusResponseEnum.Fail;

        }
    }
}