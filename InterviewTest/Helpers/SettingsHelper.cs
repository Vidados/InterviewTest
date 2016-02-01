using System.Collections.Generic;
using System.Web.Mvc;
using InterviewTest.Models;
using InterviewTest.Database;
using System.Linq;

namespace InterviewTest.Helpers
{
    public static class SettingsHelper
    {
        public static Settings GetSettings()
        {
            var db = GetDatabase();

            //use the default layout if the user hasn't defined any settings yet
            var settings = db.GetAll<Settings>().LastOrDefault() ?? new Settings() { Layout = GetDefaultLayout() };
            
            //in order to guarantee that all newsletters have a persisted layout defined, the default layout will be stored in the database
            if(settings.Id == null)
            {
                db.Save(settings);
            }

            return settings;
        }
        
        public static IEnumerable<SelectListItem> GetNewsletterItems()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Trip", Value = "" + (int)NewsletterItem.Trip },
                new SelectListItem() { Text = "Host", Value = "" + (int)NewsletterItem.Host }
            };
        }

        public static IEnumerable<SelectListItem> GetItemsAllowed()
        {
            var list = new List<SelectListItem>();
            for (var i = MinItemsAllowed; i <= MaxItemsAllowed; i++)
            {
                if (i % 2 == 0)
                {
                    list.Add(new SelectListItem() { Text = "" + i, Value = "" + i });
                }
            }
            return list;
        }

        private static IList<NewsletterItem> GetDefaultLayout()
        {
            return new List<NewsletterItem>() { NewsletterItem.Trip, NewsletterItem.Trip, NewsletterItem.Host, NewsletterItem.Host };
        }

        private static FileSystemDatabase GetDatabase() => new FileSystemDatabase();

        //as a side note, on a complex project I wouldn't define constants on helper classes,
        //but since this a very small project I'm using this helper class to define global settings
        public const int MinItemsAllowed = 4;
        public const int MaxItemsAllowed = 10;
    }
}