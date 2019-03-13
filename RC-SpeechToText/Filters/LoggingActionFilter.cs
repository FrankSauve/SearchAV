using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Filters
{
    public class LoggingActionFilter : ActionFilterAttribute
{
        private readonly ILogger _logger;

        public LoggingActionFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("LoggingControllers");

    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
            //Getting routedata for Routing and controller info
            var routeInfo = context.RouteData.Values;

            //Retrieve User IP
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();

            //Retrieve arguments and have them as string
            var arguments = context.ActionArguments;
            var argumentsString = string.Join("; " , arguments.Select(x => x.Key + "=" + x.Value).ToArray());

            //Retrieve Route that was called
            var urlHelper = new UrlHelper(context);
            var route = urlHelper.RouteUrl(context.RouteData.Values);

            //Retrieve Email of user who made call//Make sure a user is connected
            var emailClaim = context.HttpContext.User.Claims;
            string email;
            if (emailClaim.Any())
            {
                email = emailClaim.First(c => c.Type == "email").Value;
            }
            else
            {
                email = "User not currently logged in.";
            }

            //Retrieve Controller Name and Method
            var controllerName = routeInfo["Controller"];
            var controllerAction = routeInfo["Action"];

            _logger.LogInformation("Action Executing with [" +
                "\nIP: " + ip + 
                "\nEmail: " + email +
                "\nRoute Called: " + route +
                "\nController: " + controllerName + "   Action: " + controllerAction +
                "\nArguments: " + argumentsString + "]"
                );

            base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("ClassFilter OnActionExecuted");
        base.OnActionExecuted(context);
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        _logger.LogInformation("ClassFilter OnResultExecuting");
        base.OnResultExecuting(context);
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        _logger.LogInformation("ClassFilter OnResultExecuted");
        base.OnResultExecuted(context);
    }
}
}
