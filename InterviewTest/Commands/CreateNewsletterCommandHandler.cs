using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;
using InterviewTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewTest.Commands
{
    public class CreateNewsletterCommandHandler : ICommandHandler<CreateNewsletterCommand>
    {
        private readonly IDatabase _database;
        private readonly INewsletterCompositionSpecificationParserService _parser;

        public CreateNewsletterCommandHandler(IDatabase database, INewsletterCompositionSpecificationParserService parser)
        {
            _database = database;
            _parser = parser;
        }

        public void Handle(CreateNewsletterCommand command)
        {
            var currentSpecification = _database.GetAll<NewsletterCompositionSpecification>().FirstOrDefault();
            var newSpecification = _parser.Parse(command.Specification);

            if (currentSpecification == null || !currentSpecification.Equals(newSpecification))
            {
                _database.DeleteAll<NewsletterCompositionSpecification>();
                _database.Save(newSpecification);

                currentSpecification = newSpecification;
            }

            var itemIds = GetWeightedItemIds();

            for (int i = 0; i < command.Count; i++)
            {
                var items = new List<NewsletterItem>();

                foreach (var element in currentSpecification)
                {
                    var item = new NewsletterItem
                    {
                        Type = element.Type,
                        Ids = new List<string>(element.Count)
                    };
                    items.Add(item);
                    for (var j = 0; j < element.Count; j++)
                    {
                        item.Ids.Add(itemIds[element.Type].GetRandom());
                    }
                }

                _database.Save(new Newsletter { Items = items });
            }
        }

        internal Dictionary<NewsletterItemType, string[]> GetWeightedItemIds()
        {
            var newsletterItemFrequencyCounts = (from newsletter in _database.GetAll<Newsletter>()
                                                 from item in newsletter.Items
                                                 group item by item.Type into itemGroup
                                                 select new
                                                 {
                                                     Type = itemGroup.Key,
                                                     Items = (from newsletter in itemGroup
                                                              from id in newsletter.Ids
                                                              group id by id into idGroup
                                                              select new
                                                              {
                                                                  Id = idGroup.Key,
                                                                  Count = idGroup.Count()
                                                              }).ToList()
                                                 }).ToDictionary(item => item.Type);

            var hostIds = _database.GetAll<Host>().Select(h => h.Id);
            var tripIds = _database.GetAll<Trip>().Select(t => t.Id);
            var counts = new Dictionary<NewsletterItemType, int>
            {
                [NewsletterItemType.Host] = newsletterItemFrequencyCounts[NewsletterItemType.Host].Items.Sum(item => item.Count),
                [NewsletterItemType.Trip] = newsletterItemFrequencyCounts[NewsletterItemType.Trip].Items.Sum(item => item.Count)
            };

            // For each newsletter item type, create a weighted list of ids
            // Items in each list that have featured less frequently in previous newsletters will be given a higher weighing,
            // making it more likely that they will be randomly chosen when the next set of newsletters is generated

            Func<IEnumerable<string>, NewsletterItemType, string[]> getWeightedIdList = (ids, type) =>
                (from hostId in ids
                 join hostIdFrequency in newsletterItemFrequencyCounts[type].Items
                 on hostId equals hostIdFrequency.Id into matches
                 from match in matches.DefaultIfEmpty()
                 let weighting = (int)Math.Ceiling(100 - (((match?.Count ?? 0) / (decimal)counts[type]) * 100))
                 from i in Enumerable.Range(1, weighting)
                 select hostId).ToArray();

            return new Dictionary<NewsletterItemType, string[]>
            {
                [NewsletterItemType.Host] = getWeightedIdList(hostIds, NewsletterItemType.Host),
                [NewsletterItemType.Trip] = getWeightedIdList(tripIds, NewsletterItemType.Trip)
            };
        }
    }
}