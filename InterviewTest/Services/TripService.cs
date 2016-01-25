using InterviewTest.Database;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public class TripService : ITripService
    {
        private readonly IDatabase _database;

        public TripService(IDatabase database)
        {
            _database = database;
        }

        public Trip GetById(string id)
        {
            return _database.Get<Trip>(id);
        }
    }
}