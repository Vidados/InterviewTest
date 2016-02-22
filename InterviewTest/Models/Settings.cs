using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Settings : IPersistable
    {
        public Settings()
        {}

        public Settings(string key) : base()
        {
            Key = key;
        }

        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }        
    }



}