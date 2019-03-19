using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Incoming
{
    public class ControllerErrorDTO
{
    public string message { get; set; }
    public bool isError { get; set; }
    public string detail { get; set; }

    public ControllerErrorDTO(string message)
    {
        this.message = message;
        isError = true;
    }
    public ControllerErrorDTO(string message, string detail)
    {
        this.message = message;
        this.detail = detail;
        isError = true;
    }
}
}
