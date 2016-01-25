using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public class NewsletterService: INewsletterService
    {
        private IFileSystemDatabase _fileSystemDatabase;

        public NewsletterService(IFileSystemDatabase fileSystemDatabase)
        {
            _fileSystemDatabase = fileSystemDatabase;
        }

        public void CreateRandomData(int count)
        {
            var hostIds = _fileSystemDatabase.GetAll<Host>().Select(h => h.Id).ToArray();
            var tripIds = _fileSystemDatabase.GetAll<Trip>().Select(t => t.Id).ToArray();

            for (int i = 0; i < count; i++)
            {
                var newsletter = new Newsletter()
                {
                    HostIds = Enumerable.Range(0, 2).Select(x => hostIds.GetRandom()).ToList(),
                    TripIds = Enumerable.Range(0, 2).Select(x => tripIds.GetRandom()).ToList(),
                };

                _fileSystemDatabase.Save(newsletter);
            }
        }

        public void DeleteAll()
        {
            _fileSystemDatabase.DeleteAll<Newsletter>();
        }

        public Newsletter GetById(string id)
        {
            return _fileSystemDatabase.Get<Newsletter>(id);
        }

        public List<Newsletter> GetAll()
        {
            return _fileSystemDatabase.GetAll<Newsletter>();
        }
    }
}