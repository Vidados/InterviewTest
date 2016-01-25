using InterviewTest.Models;

namespace InterviewTest.Services
{
    public interface ITripService
    {
        Trip GetById(string id);
    }
}