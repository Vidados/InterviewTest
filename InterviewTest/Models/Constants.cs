using InterviewTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterviewTest.Models
{
    public static class Constants
    {
        public const char TripIdentifier = 'T';
        public const char HostIdentifier = 'H';

        public struct SettingKeys
        {
            public const string NewsLetterFormatKey = "NewsLetterFormat";
        }

        public static Dictionary<string, string> DefaultInitValues => 
            new Dictionary<string, string> { { SettingKeys.NewsLetterFormatKey, "HHTT" } };
    }
}