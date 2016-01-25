using InterviewTest.Database;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public class HostService : IHostService
    {
        private readonly IFileSystemDatabase _fileSystemDatabase;

        public HostService(IFileSystemDatabase fileSystemDatabase)
        {
            _fileSystemDatabase = fileSystemDatabase;
        }

        public Host GetById(string id)
        {
            return _fileSystemDatabase.Get<Host>(id);
        }
    }
}