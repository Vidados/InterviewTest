using System;
using System.Collections.Generic;
using System.Linq;
using InterviewTest.Controllers;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Services;
using NUnit.Framework;

namespace InterviewTest.Tests.Controllers
{
    [TestFixture]
    public class NewsletterTests
    {
        private NewsletterVmService _nlVmServ;

        [SetUp]
        public void Setup()
        {
            _nlVmServ = new NewsletterVmService(new FileSystemDatabase());
        }

        [Test]
        public void ShouldCreateNewsletterViewModelWithCorrectNoOfItems()
        {
            var dummyIds = new List<string> { "00000", "00001", "00002" };
            var nViewModel = _nlVmServ.GetNewsletterViewModel(new Newsletter
            {
                ConfigId = "00000",
                CreatedAt = DateTime.Now,
                HostIds = dummyIds,
                Id = "00000",
                TripIds = dummyIds
            });

            Assert.That(nViewModel, Is.Not.Null);
            Assert.That(nViewModel.Items.Count, Is.EqualTo(4));
        }

        [Test]
        public void ShouldCreateNewsletterViewModelBasedOnConfig()
        {
            var dummyIds = new List<string> { "00000", "00001", "00002" };
            var nViewModel = _nlVmServ.GetNewsletterViewModel(new Newsletter
            {
                ConfigId = "00000",
                CreatedAt = DateTime.Now,
                HostIds = dummyIds,
                Id = "00000",
                TripIds = dummyIds
            });

            Assert.That(nViewModel.Items.Count(i => i is NewsletterTripViewModel), Is.EqualTo(2));
            Assert.That(nViewModel.Items.Count(i => i is NewsletterHostViewModel), Is.EqualTo(2));
        }

        [Test]
        public void ShouldCreateNewsletterViewModelBasedOnConfig2()
        {
            var dummyIds = new List<string> { "00000", "00001", "00002" };
            var nViewModel = _nlVmServ.GetNewsletterViewModel(new Newsletter
            {
                ConfigId = "00000",
                CreatedAt = DateTime.Now,
                HostIds = dummyIds,
                Id = "00000",
                TripIds = dummyIds
            }, new ConfigModel("TTT"));

            Assert.That(nViewModel.Items.Count(i => i is NewsletterTripViewModel), Is.EqualTo(3));
            Assert.That(nViewModel.Items.Count(i => i is NewsletterHostViewModel), Is.EqualTo(0));
        }

        [Test]
        public void ShouldCreateNewsletterViewModelInCorrectOrder()
        {
            var dummyIds = new List<string> { "00000", "00001", "00002" };
            var nViewModel = _nlVmServ.GetNewsletterViewModel(new Newsletter
            {
                ConfigId = "00000",
                CreatedAt = DateTime.Now,
                HostIds = dummyIds,
                Id = "00000",
                TripIds = dummyIds
            }, new ConfigModel("TTH"));

            Assert.That(nViewModel.Items.ElementAt(0), Is.InstanceOf(typeof(NewsletterTripViewModel)));
            Assert.That(nViewModel.Items.ElementAt(1), Is.InstanceOf(typeof(NewsletterTripViewModel)));
            Assert.That(nViewModel.Items.ElementAt(2), Is.InstanceOf(typeof(NewsletterHostViewModel)));
        }
    }
}
