using InterviewTest.Database;
using System.Collections.ObjectModel;
using System.Text;
using System;
using System.Linq;

namespace InterviewTest.Models
{
    public class NewsletterCompositionSpecificationElement
    {
        public NewsletterItemType Type { get; set; }
        public int Count { get; set; }

        public override bool Equals(object obj)
        {
            var element = obj as NewsletterCompositionSpecificationElement;
            if (obj == null) return false;

            return Type == element.Type && Count == element.Count;
        }

        public override int GetHashCode()
        {
            return (int)Type ^ Count;
        }

        public override string ToString()
        {
            var character = Type == NewsletterItemType.Host
                ? 'H' : 'T';

            return new string(character, Count);
        }
    }

    public class NewsletterCompositionSpecification 
        : Collection<NewsletterCompositionSpecificationElement>, IPersistable
    {
        public string Id
        {
            get
            {
                return ToString();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var item in Items)
            {
                builder.Append(item.ToString());
            }

            return builder.ToString();
        }
    }
}