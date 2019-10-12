using DotNetCore30Demo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetCore30Demo.Utility
{
    public class ValidationFailedResult:ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState) : base(new ValidationFailedResultResponse(modelState))
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }
    }
}