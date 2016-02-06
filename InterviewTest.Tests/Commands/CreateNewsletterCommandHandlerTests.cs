using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterviewTest.Commands;
using NSubstitute;
using InterviewTest.Database;
using InterviewTest.Services;
using InterviewTest.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace InterviewTest.Tests.Commands
{
    [TestClass]
    public class CreateNewsletterCommandHandlerTests
    {
        private NewsletterCompositionSpecification _currentSpecification;
        private NewsletterCompositionSpecification _newSpecification;
        private IDatabase _mockDatabase;
        private INewsletterCompositionSpecificationParserService _mockParser;
        private CreateNewsletterCommandHandler _handler;
        private List<Newsletter> _newsletters;

        [TestInitialize]
        public void TestInitialize()
        {
            _currentSpecification = new NewsletterCompositionSpecification
            {
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Host,
                    Count = 3
                },
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Trip,
                    Count = 2
                },
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Host,
                    Count = 3
                }
            };
            _newSpecification = new NewsletterCompositionSpecification
            {
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Host,
                    Count = 2
                },
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Trip,
                    Count = 2
                },
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Host,
                    Count = 2
                }
            };
            _newsletters = new List<Newsletter>
            {
                new Newsletter
                {
                    Items = new List<NewsletterItem>
                    {
                        new NewsletterItem
                        {
                            Ids = new List<string> {"1","2","3"},
                            Type = NewsletterItemType.Host
                        },
                        new NewsletterItem
                        {
                            Ids = new List<string> {"1","2","3"},
                            Type = NewsletterItemType.Trip
                        }
                    }
                },
                new Newsletter
                {
                    Items = new List<NewsletterItem>
                    {
                        new NewsletterItem
                        {
                            Ids = new List<string> {"1","2","4"},
                            Type = NewsletterItemType.Host
                        },
                        new NewsletterItem
                        {
                            Ids = new List<string> {"2","4","6"},
                            Type = NewsletterItemType.Trip
                        }
                    }
                }
            };

            _mockDatabase = Substitute.For<IDatabase>();
            _mockDatabase.GetAll<NewsletterCompositionSpecification>().Returns(
                new List<NewsletterCompositionSpecification> { _currentSpecification });
            _mockDatabase.GetAll<Host>().Returns(Enumerable.Range(1, 10).Select(i => new Host { Id = i.ToString() }).ToList());
            _mockDatabase.GetAll<Trip>().Returns(Enumerable.Range(1, 10).Select(i => new Trip { Id = i.ToString() }).ToList());
            _mockDatabase.GetAll<Newsletter>().Returns(_newsletters);

            _mockParser = Substitute.For<INewsletterCompositionSpecificationParserService>();
            _mockParser.Parse(_currentSpecification.ToString()).Returns(_currentSpecification);
            _mockParser.Parse(_newSpecification.ToString()).Returns(_newSpecification);

            _handler = new CreateNewsletterCommandHandler(_mockDatabase, _mockParser);
        }

        [TestMethod]
        public void ShouldSaveSpecificationIfItHasChanged()
        {
            _handler.Handle(new CreateNewsletterCommand
            {
                Count = 5,
                Specification = _newSpecification.ToString()
            });

            _mockDatabase.Received().DeleteAll<NewsletterCompositionSpecification>();
            _mockDatabase.Received().Save(_newSpecification);
        }

        [TestMethod]
        public void ShouldSaveSpecificationIfOneDoesNotExist()
        {
            _mockDatabase.GetAll<NewsletterCompositionSpecification>().Returns(
                new List<NewsletterCompositionSpecification>());

            _handler.Handle(new CreateNewsletterCommand
            {
                Count = 5,
                Specification = _newSpecification.ToString()
            });

            _mockDatabase.Received().DeleteAll<NewsletterCompositionSpecification>();
            _mockDatabase.Received().Save(_newSpecification);
        }

        [TestMethod]
        public void ShouldNotSaveSpecificationIfItHasNotChanged()
        {
            _handler.Handle(new CreateNewsletterCommand
            {
                Count = 5,
                Specification = _currentSpecification.ToString()
            });

            _mockDatabase.DidNotReceive().DeleteAll<NewsletterCompositionSpecification>();
            _mockDatabase.DidNotReceive().Save(_currentSpecification);
        }

        [TestMethod]
        public void ShouldCreateNewslettersWhereItemsMatchSpecification()
        {
            _handler.Handle(new CreateNewsletterCommand
            {
                Count = 5,
                Specification = _currentSpecification.ToString()
            });

            _mockDatabase.Received().Save(Arg.Is<Newsletter>(x => 
                _currentSpecification.Zip(x.Items, (element, item) => new { element, item })
                .All(pair => pair.element.Type == pair.item.Type && pair.element.Count == pair.item.Ids.Count)));
        }

        [TestMethod]
        public void GetWeightedItemIdsShouldReturnAHigherWeightingForHostsThatHaveFeaturedLessFrequentlyOnPreviousNewsletters()
        {
            var weightedIds = _handler.GetWeightedItemIds()[NewsletterItemType.Host];

            Assert.AreEqual(100, weightedIds.Count(id => id == "5"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "6"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "7"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "8"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "9"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "10"));
            Assert.AreEqual(84, weightedIds.Count(id => id == "4"));
            Assert.AreEqual(84, weightedIds.Count(id => id == "3"));
            Assert.AreEqual(67, weightedIds.Count(id => id == "2"));
            Assert.AreEqual(67, weightedIds.Count(id => id == "1"));
        }

        [TestMethod]
        public void GetWeightedItemIdsShouldReturnAHigherWeightingForTripsThatHaveFeaturedLessFrequentlyOnPreviousNewsletters()
        {
            var weightedIds = _handler.GetWeightedItemIds()[NewsletterItemType.Trip];

            Assert.AreEqual(100, weightedIds.Count(id => id == "5"));
            Assert.AreEqual(84, weightedIds.Count(id => id == "6"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "7"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "8"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "9"));
            Assert.AreEqual(100, weightedIds.Count(id => id == "10"));
            Assert.AreEqual(84, weightedIds.Count(id => id == "4"));
            Assert.AreEqual(84, weightedIds.Count(id => id == "3"));
            Assert.AreEqual(67, weightedIds.Count(id => id == "2"));
            Assert.AreEqual(84, weightedIds.Count(id => id == "1"));
        }
    }
}