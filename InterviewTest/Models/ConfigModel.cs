using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class ConfigModel : IPersistable
    {
        public string Id { get; set; }
        public IEnumerable<char> ConfigTokens { get; set; }

        public ConfigModel() { } 

        public ConfigModel(string configString)
        {
            ConfigTokens = configString.ToCharArray();
        }

        public ConfigModel(string id, string configString)
        {
            Id = id;
            ConfigTokens = configString.ToCharArray();
        }

        public override string ToString()
        {
            return string.Join("", ConfigTokens);
        }
    }
}