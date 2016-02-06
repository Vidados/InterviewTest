using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;
using InterviewTest.Services;
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

            var hostIds = _database.GetAll<Host>().Select(h => h.Id).ToArray();
            var tripIds = _database.GetAll<Trip>().Select(t => t.Id).ToArray();

            for (int i = 0; i < command.Count; i++)
            {
                var items = (from element in currentSpecification
                             let ids = element.Type == NewsletterItemType.Host ? hostIds : tripIds
                             select new NewsletterItem
                             {
                                 Type = element.Type,
                                 Ids = Enumerable.Range(1, element.Count).Select(_ => ids.GetRandom()).ToList()
                             }).ToList();
                var newsletter = new Newsletter()
                {
                    Items = items
                };

                _database.Save(newsletter);
            }
        }
    }
}