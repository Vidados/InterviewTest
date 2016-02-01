using System;
using System.Collections.Generic;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Config : IPersistable
    {
        public string Id { get; set; }
        public string Sequence { get; set; }
    }
}