using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;
using Microsoft.Ajax.Utilities;

namespace InterviewTest.Services
{
    public class NewsletterService: INewsletterService
    {
        private readonly IDatabase _database;

        public NewsletterService(IDatabase database)
        {
            _database = database;
        }

        public void CreateRandomData(int count, string pattern)
        {
            var hostIds = _database.GetAll<Host>().Select(h => h.Id).ToArray();
            var tripIds = _database.GetAll<Trip>().Select(t => t.Id).ToArray();

            var hostCount = pattern.ToCharArray().Where(x => x.Equals('H')).Count();
            var tripCount = pattern.ToCharArray().Where(x => x.Equals('T')).Count();
            for (int i = 0; i < count; i++)
            {
                var newsletter = new Newsletter()
                {
                    HostIds = Enumerable.Range(0, hostCount).Select(x => hostIds.GetRandom()).ToList(),
                    TripIds = Enumerable.Range(0, tripCount).Select(x => tripIds.GetRandom()).ToList(),
                };

                newsletter.DisplayPattern = pattern;
                _database.Save(newsletter);
            }
        }

        public List<IContent> GetContents(Newsletter newsletter)
        {
            var hostIndex = 0;
            var tripIndex = 0;
            var contents =new List<IContent>();

            foreach (var contentType in newsletter.DisplayPattern.ToCharArray())
            {
                IContent content = null;
                switch (contentType)
                {
                    case 'H':
                        if (hostIndex >= newsletter.HostIds.Count)
                            continue;
                        content = _database.Get<Host>(newsletter.HostIds[hostIndex]);
                        hostIndex++;
                        break;
                    case 'T':
                        if (tripIndex >= newsletter.TripIds.Count)
                            continue;
                        content = _database.Get<Trip>(newsletter.TripIds[tripIndex]);
                        tripIndex++;
                        break;
                }
                contents.Add(content);
            }
            return contents;
        }
        
        public void DeleteAll()
        {
            _database.DeleteAll<Newsletter>();
        }

        public Newsletter GetById(string id)
        {
            return _database.Get<Newsletter>(id);
        }
        
        public List<Newsletter> GetAll()
        {
            return _database.GetAll<Newsletter>();
        }
    }
}