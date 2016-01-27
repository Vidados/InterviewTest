using System;
using System.Collections.Generic;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Layoutsplits : IPersistable
    {
     
        public List<List<string>> layoutOf { get; set; }
        public string Id { get; set; }


    }
}