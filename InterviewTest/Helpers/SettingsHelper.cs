using System.Collections.Generic;
using System.Web.Mvc;
using InterviewTest.Models;
using InterviewTest.Database;
using System.Linq;

namespace InterviewTest.Helpers
{
    public static class SettingsHelper
    {
        private static IList<NewsletterItem> GetDefaultLayout()
        {
            return new List<NewsletterItem>() { NewsletterItem.Trip, NewsletterItem.Trip, NewsletterItem.Host, NewsletterItem.Host };
        }

        public static Settings GetSettings(FileSystemDatabase db)
        {
            //use default layout if the newsletter has no settings defined
            return db.GetAll<Settings>().LastOrDefault() ?? new Settings() { Layout = GetDefaultLayout() };
        }

        public static Settings GetSettings(FileSystemDatabase db, string id)
        {
            //use default layout if there are no settings defined with the given id
            return db.Get<Settings>(id) ?? new Settings() { Layout = GetDefaultLayout() };
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

        //as a side note, on a complex project I wouldn't define constants on helper classes,
        //but since this a very small project I'm using this helper class to define global settings
        public const int MinItemsAllowed = 4;
        public const int MaxItemsAllowed = 10;
    }
}