using System;
using DotNetCore30Demo.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCore30Demo.Utility
{
    public class CustomExceptionResult : ObjectResult
    {
        public CustomExceptionResult(int? code, Exception exception) : base(new CustomExceptionResponse(code, exception))
        {
        }
    }
}