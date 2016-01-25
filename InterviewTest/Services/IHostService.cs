using InterviewTest.Models;

namespace InterviewTest.Services
{
    public interface IHostService
    {
        Host GetById(string id);
    }
}