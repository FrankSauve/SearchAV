using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Filters
{
    public class ControllerExceptionFilter : ExceptionFilterAttribute
{
        private readonly ILogger _logger;

        public ControllerExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("ControllerException");

        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError("Exception thrown");
            base.OnException(context);
        }
    }
}
