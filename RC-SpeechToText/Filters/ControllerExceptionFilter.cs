﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using RC_SpeechToText.Exceptions;
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
            //Retrieve User IP
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();

            //Retrieve Route that was called
            var urlHelper = new UrlHelper(context);
            var route = urlHelper.RouteUrl(context.RouteData.Values);

            //Get arguments that caused the exception
            var arguments = context.ActionDescriptor.RouteValues;
            var argumentsString = string.Join("; ", arguments.Select(x => x.Key + "=" + x.Value).ToArray());

            ControllerError controllerError = null;
            //Handling known Controller errors
            if (context.Exception is ControllerExceptions)
            {
                // Handling controller errors 
                var ex = context.Exception as ControllerExceptions;
                context.Exception = null;
                controllerError = new ControllerError(ex.Message, ex.StackTrace);

                context.HttpContext.Response.StatusCode = ex.StatusCode;

            }
            else
            {
                // Handling unexpected errors
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;

                controllerError = new ControllerError(msg, stack);
                context.HttpContext.Response.StatusCode = 500;
            }

            _logger.LogError("Action Executing with [" +
                "\nIP: " + ip +
                "\nRoute Called: " + route +
                "\nArguments: " + argumentsString +
                "\nException: " + controllerError.message +
                "\nTrace: " + controllerError.detail + "]"
                );

            context.Result = new JsonResult(controllerError);

            base.OnException(context);
        }
    }
}
