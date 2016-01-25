using InterviewTest.Database;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public class TripService : ITripService
    {
        private readonly IFileSystemDatabase _fileSystemDatabase;

        public TripService(IFileSystemDatabase fileSystemDatabase)
        {
            _fileSystemDatabase = fileSystemDatabase;
        }

        public Trip GetById(string id)
        {
            return _fileSystemDatabase.Get<Trip>(id);
        }
    }
}