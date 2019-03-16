using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Exceptions
{
    public class ControllerExceptions : Exception
{
        public int StatusCode { get; set; }

        public ControllerExceptions(string message) : base(message)
        {
            StatusCode = 400;
        }
        
    }

    //To generate JSON that will be passed to client.
    public class ControllerError
    {
        public string message { get; set; }
        public bool isError { get; set; }
        public string detail { get; set; }

        public ControllerError(string message)
        {
            this.message = message;
            isError = true;
        }
        public ControllerError(string message, string detail)
        {
            this.message = message;
            this.detail = detail;
            isError = true;
        }

    }

}

