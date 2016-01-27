using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {

        private char[] typesOf;
        static Layoutsplits splits = new Layoutsplits();

        public string getString()
        {
            var db = GetDatabase();
            splitsText sp = db.Get<splitsText>("00000");
            if (sp == null)
            {
                return "TTHHTT";
            }
            return sp.layoutString;
        }

        public ActionResult Create(int count, string formatter)
        {
            if(!formatter.Contains("T") || !formatter.Contains("H"))
            {
                TempData["notification"] = $"Couldn't make newsletters as you were missing T or H";

                return RedirectToAction("list");

            }
            var db = GetDatabase();

            splits.layoutOf = new List<List<string>>();

            typesOf = formatter.ToCharArray();
            splitsText sp = new splitsText();
            sp.Id = "00000";
            sp.layoutString = formatter;
            db.Save<splitsText>(sp);


           
            for (int i = 0; i < count; i++)
            {
                splits.layoutOf = new List<List<string>>();
                string[] hostIds = db.GetAll<Host>().OrderBy(h => h.used).Select(h => h.Id).Take(9).ToArray();
                string[] tripIds = db.GetAll<Trip>().OrderBy(t => t.used).Select(t => t.Id).Take(9).ToArray();
                // only grab top 9 records so most popular isnt reused until its been caught up with.

                int counter = 1;
                for (int ii = 0; ii < typesOf.Length; ii++)
                {
                    List<string> thissplit = new List<string>();
                    thissplit.Add(counter.ToString());
                    thissplit.Add(typesOf[ii] == "T"[0] ? "0" : "1");
                    thissplit.AddRange(typesOf[ii] == "T"[0] ? 
                        Enumerable.Range(0, counter).Select(x => tripIds.GetRandom()).ToList() : 
                        Enumerable.Range(0, counter).Select(x => hostIds.GetRandom()).ToList());
                    if (typesOf[ii] == "T"[0])
                    {
                        List<string> list = new List<string>(tripIds);
                        list.Remove(thissplit[2]); // remove ids that have been used already so to not have duplication in the newsletter
                        tripIds = list.ToArray();
                    }
                    else
                    {
                        List<string> list = new List<string>(hostIds);
                        list.Remove(thissplit[2]); // remove ids that have been used already so to not have duplication in the newsletter
                        hostIds = list.ToArray();
                    }
                    splits.layoutOf.Add(thissplit);
                    if (typesOf[ii] == "T"[0])
                    {
                        Trip tt = db.Get<Trip>(thissplit[2]);
                        tt.used += 1;
                        db.Save<Trip>(tt);
                    }
                    else
                    {
                        Host hh = db.Get<Host>(thissplit[2]);
                        hh.used += 1;
                        db.Save<Host>(hh);
                    }


                }

                var newsletter = new Newsletter()
                {
                    LayoutOf = splits.layoutOf,

                    //HostIds = Enumerable.Range(0, 2).Select(x => hostIds.GetRandom()).ToList(),
                    //TripIds = Enumerable.Range(0, 2).Select(x => tripIds.GetRandom()).ToList(),

                };
                splits.Id = newsletter.Id;
                db.Save(splits);

                db.Save(newsletter);

            }

            TempData["notification"] = $"Created {count} newsletters";

            return RedirectToAction("list");
        }

        public List<string> getCountsT()
        {
            List<string> l = new List<string>();
            var db = GetDatabase();

            List<Trip> t = db.GetAll<Trip>();
            foreach (Trip tr in t)
            {
                l.Add(tr.Id + ": " + tr.used);
            }

            return l;
        }
        public List<string> getCountsH()
        {
            List<string> l = new List<string>();
            var db = GetDatabase();
            List<Host> h = db.GetAll<Host>();

            foreach (Host ho in h)
            {
                l.Add(ho.Id + ": " + ho.used);

            }
            return l;
        }
        public ActionResult DeleteAll()
        {
            GetDatabase().DeleteAll<Newsletter>();
            GetDatabase().DeleteAll<Layoutsplits>();

            TempData["notification"] = "All newsletters deleted";

            return RedirectToAction("list");
        }

        private List<object> res(Newsletter newsletter, FileSystemDatabase db)
        {
            List<object> Res = new List<object>();
            Layoutsplits lay = db.Get<Layoutsplits>(newsletter.Id);
            splits.layoutOf = lay.layoutOf;


            foreach (List<string> l in splits.layoutOf)
            {
            
                switch (l[1])
                {
                    case "0": //trip

                        NewsletterTripViewModel t = Convert((Trip)db.Get<Trip>(l[2]));
                        Res.Add(t);

                        break;
                    default: // host
                        NewsletterHostViewModel t2 = Convert((Host)db.Get<Host>(l[2]));
                        Res.Add(t2);

                        break;

                }
            
            }

            return Res;
        }

        public ActionResult Display(string id)
        {
            var db = GetDatabase();

            var newsletter = db.Get<Newsletter>(id);


            var viewModel = new NewsletterViewModel
            {
                //Items = newsletter.TripIds.Select(tid => Convert(db.Get<Trip>(tid))).Cast<object>()
                //    .Union(newsletter.HostIds.Select(hid => Convert(db.Get<Host>(hid))))
                //    .ToList(),
                Items = res(newsletter, db),

            };

            return View("Newsletter", viewModel);
        }

        public ActionResult Sample()
        {
            var viewModel = new NewsletterViewModel()
            {
                Items =
                {
                    Convert(EntityGenerator.GenerateHost()),
                    Convert(EntityGenerator.GenerateTrip(null), "Test host name"),
                    Convert(EntityGenerator.GenerateTrip(null), "Test host name"),
                    Convert(EntityGenerator.GenerateHost()),
                }
            };

            return View("Newsletter", viewModel);
        }

        public ActionResult List()
        {
            var viewModel = new NewsletterListViewModel
            {
                Newsletters = GetDatabase().GetAll<Newsletter>(),
                setString = getString(),
                res = getCountsT(),
                res2 = getCountsH(),

            };

            return View(viewModel);
        }

        private NewsletterHostViewModel Convert(Host host) => new NewsletterHostViewModel
        {
            Name = host.Name,
            ImageUrl = host.ImageUrl,
            Job = host.Job,
            addclass = host.addclass,
        };

        private NewsletterTripViewModel Convert(Trip trip, string hostName = null) => new NewsletterTripViewModel
        {
            Name = trip.Name,
            Country = trip.Country,
            HostName = hostName ?? GetDatabase().Get<Host>(trip.HostId)?.Name,
            ImageUrl = trip.ImageUrl,
            addclass = trip.addclass,
        };

        private FileSystemDatabase GetDatabase() => new FileSystemDatabase();

    }

    public class NewsletterViewModel
    {
        public List<object> Items { get; set; } = new List<object>();
    }

    public class NewsletterTripViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Country { get; set; }
        public string HostName { get; set; }
        public string ImageUrl { get; set; }
        public string addclass { get; set; }
    }

    public class NewsletterHostViewModel
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string ImageUrl { get; set; }
        public string addclass { get; set; }
    }

    public class NewsletterListViewModel
    {
        public List<string> res { get; set; }
        public List<string> res2 { get; set; }
        public List<Newsletter> Newsletters { get; set; }
        public string setString { get; set; }
    }
}