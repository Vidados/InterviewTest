using InterviewTest.Database;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public class HostService : IHostService
    {
        private readonly IDatabase _database;

        public HostService(IDatabase database)
        {
            _database = database;
        }

        public Host GetById(string id)
        {
            return _database.Get<Host>(id);
        }
    }
}