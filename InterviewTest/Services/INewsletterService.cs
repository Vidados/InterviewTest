using System.Collections;
using System.Collections.Generic;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public interface INewsletterService
    {
        void CreateRandomData(int count, string pattern);
        void DeleteAll();
        Newsletter GetById(string id);
        List<Newsletter> GetAll();
        List<IContent> GetContents(Newsletter newsletter);
    }
}