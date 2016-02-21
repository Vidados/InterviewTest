using System;
using System.Collections.Generic;
using System.Linq;
using InterviewTest.Controllers;
using InterviewTest.Database;
using InterviewTest.Models;

namespace InterviewTest.Services
{
    public class NewsletterVmService
    {
        private readonly FileSystemDatabase _db;

        public NewsletterVmService(FileSystemDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _db = db;
        }

        public NewsletterViewModel GetNewsletterViewModel(Newsletter newsletter)
        {
            return GetNewsletterViewModel(newsletter, null);
        }

        public NewsletterViewModel GetNewsletterViewModel(Newsletter newsletter, ConfigModel configModel = null)
        {
            var newsletterVm = new NewsletterViewModel();
            var trips = new Queue<NewsletterTripViewModel>(newsletter.TripIds.Select(tid => Convert(_db.Get<Trip>(tid))));
            var hosts = new Queue<NewsletterHostViewModel>(newsletter.HostIds.Select(hid => Convert(_db.Get<Host>(hid))));
            var config = configModel ?? _db.Get<ConfigModel>(newsletter.ConfigId);

            foreach (var token in config.ConfigTokens)
            {
                if (token == 'T') newsletterVm.Items.Add(trips.Dequeue());
                if (token == 'H') newsletterVm.Items.Add(hosts.Dequeue());
            }

            return newsletterVm;
        }

        public NewsletterViewModel GetSampleNewsletter()
        {
            return new NewsletterViewModel()
            {
                Items =
                {
                    Convert(EntityGenerator.GenerateHost()),
                    Convert(EntityGenerator.GenerateTrip(null), "Test host name"),
                    Convert(EntityGenerator.GenerateTrip(null), "Test host name"),
                    Convert(EntityGenerator.GenerateHost()),
                }
            };
        }

        private NewsletterHostViewModel Convert(Host host) => new NewsletterHostViewModel
        {
            Name = host.Name,
            ImageUrl = host.ImageUrl,
            Job = host.Job,
        };

        private NewsletterTripViewModel Convert(Trip trip, string hostName = null) => new NewsletterTripViewModel
        {
            Name = trip.Name,
            Country = trip.Country,
            HostName = hostName ?? new FileSystemDatabase().Get<Host>(trip.HostId)?.Name,
            ImageUrl = trip.ImageUrl,
        };
    }
}