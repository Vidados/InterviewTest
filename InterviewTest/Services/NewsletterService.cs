using System;
using System.Collections.Generic;
using System.Linq;
using InterviewTest.Database;
using InterviewTest.Models;
using WebGrease.Css.Extensions;

namespace InterviewTest.Services
{
    public class NewsletterService
    {
        private readonly FileSystemDatabase _db;

        public NewsletterService(FileSystemDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _db = db;
        }

        public void SaveNewsletters(char hostsToken, char tripsToken, int count)
        {
            var newsletters = GetNewsletters(GetHostsOrderedDict(_db), GetTripsOrderedDict(_db), hostsToken, tripsToken, count);

            foreach (var newsletter in newsletters)
            {
                _db.Save(newsletter);
            }
        }

        public List<Newsletter> GetNewsletters(Dictionary<string,int> hostsList, Dictionary<string, int> tripsList, char hostsToken, char tripsToken, int count)
        {
            var newsletters = new List<Newsletter>();
            var hostsDict = GetHostsOrderedDict(_db);
            var tripsDict = GetTripsOrderedDict(_db);
            var config = _db.GetAll<ConfigModel>().OrderByDescending(x => x.Id).FirstOrDefault() ??
                         CreateAndSaveDefaultConfig(_db);

            int noHosts = config.ConfigTokens.Count(t => t.Equals(hostsToken));
            int noTrips = config.ConfigTokens.Count(t => t.Equals(tripsToken));

            for (int i = 0; i < count; i++)
            {
                var newsletter = new Newsletter
                {
                    HostIds = hostsDict.Keys.Take(noHosts).ToList(),
                    TripIds = tripsDict.Keys.Take(noTrips).ToList(),
                    ConfigId = config.Id
                };
                hostsDict = UpdateSourceDict(hostsDict, noHosts);
                tripsDict = UpdateSourceDict(tripsDict, noTrips);

                newsletters.Add(newsletter);
            }
            return newsletters;
        }

        private Dictionary<string, int> UpdateSourceDict(Dictionary<string, int> sourceDict, int noOfUpdatedItems)
        {
            for (var i = 0; i < noOfUpdatedItems; i++)
            {
                sourceDict[sourceDict.Keys.ElementAt(i)] += 1;
            }

            return sourceDict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public Dictionary<string, int> GetHostsOrderedDict(FileSystemDatabase db)
        {
            var newsletters = db.GetAll<Newsletter>();
            var hostsDict = new Dictionary<string, int>();

            foreach (var newsletter in newsletters)
            {
                foreach (var hostId in newsletter.HostIds)
                {
                    UpdateDict(hostsDict, hostId);
                }
            }

            var allMissingHosts = db.GetAll<Host>().Where(h => !hostsDict.ContainsKey(h.Id));
            allMissingHosts.ForEach(h => hostsDict[h.Id] = 0);

            return hostsDict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value); ;
        }

        public Dictionary<string,int> GetTripsOrderedDict(FileSystemDatabase db)
        {
            var newsletters = db.GetAll<Newsletter>();
            var dict = new Dictionary<string, int>();

            foreach (var newsletter in newsletters)
            {
                foreach (var hostId in newsletter.TripIds)
                {
                    UpdateDict(dict, hostId);
                }
            }

            var allMissingTrips = db.GetAll<Host>().Where(h => !dict.ContainsKey(h.Id));
            allMissingTrips.ForEach(h => dict[h.Id] = 0);

            return dict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value); ;
        }

        private void UpdateDict(Dictionary<string, int> hostsDict, string hostId)
        {
            if (hostsDict.ContainsKey(hostId))
            {
                hostsDict[hostId] += 1;
            }
            else
            {
                hostsDict[hostId] = 1;
            }
        }

        private ConfigModel CreateAndSaveDefaultConfig(FileSystemDatabase db)
        {
            var config = new ConfigModel("00000", "TTHH");

            db.Save(config);

            return config;
        }
    }
}