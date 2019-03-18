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
            StatusCode = 500;
        }
        
    }

    
}

