using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace TrueLayer.Api.Infrastructure
{
    public class ExceptionFilter : IExceptionFilter
    {
        class ExceptionViewModel
        {
            public string Message { get; set; }
            public string Detail { get; set; }
        }

        private readonly bool _showStackTraces;

        public ExceptionFilter(IHostEnvironment environment)
        {
            _showStackTraces = environment.EnvironmentName is "Development" or "Test";
        }
        
        public void OnException(ExceptionContext context)
        {
            var exceptionViewModel = _showStackTraces switch
            {
                true => new ExceptionViewModel
                {
                    Message = context.Exception.Message,
                    Detail = context.Exception.StackTrace,
                },
                false => new ExceptionViewModel
                {
                    Message = "An unexpected error occurred.",
                    Detail = context.Exception.Message
                }
            };

            var result = new ObjectResult(exceptionViewModel)
            {
                StatusCode = 500
            };

            context.Result = result;
        }
    }
}