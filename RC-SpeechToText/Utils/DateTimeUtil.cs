using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Utils
{
    public class DateTimeUtil
{
    public static string FormatDateCardInfo(string date)
    {
            DateTime dateObject = DateTime.Parse(date);
            return dateObject.ToString("MM/dd/yyyy hh:mm tt");
    }
}
}
