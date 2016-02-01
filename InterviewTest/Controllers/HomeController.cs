using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Helpers;

namespace InterviewTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private FileSystemDatabase GetDatabase() => new FileSystemDatabase();
        private static readonly Random _random = new Random();

        public ActionResult CreateTripsAndSellers()
        {
            var database = GetDatabase();

            //in order to guarantee that all newsletters have a persisted layout defined, the default layout will be stored in the database
            var settings = new Settings() { Layout = SettingsHelper.GetDefaultLayout() };
            database.Save(settings);

            //whenever a new host or trip is created its stats are set to 0
            var stats = database.GetAll<Stats>().LastOrDefault() ?? new Stats();

            for (int i = 0; i < 10; i++)
            {
                var host = EntityGenerator.GenerateHost();
                host.Id = i.ToString("00000");
                stats.Hosts[host.Id] = 0;
                database.Save(host);
            }

            for (int i = 0; i < 10; i++)
            {
                var trip = EntityGenerator.GenerateTrip(_random.Next(0, 10).ToString("00000"));
                trip.Id = i.ToString("00000");
                stats.Trips[trip.Id] = 0;
                database.Save(trip);
            }

            database.Save(stats);

            TempData["notification"] = "10 trips and 10 hosts created";

            return RedirectToAction("index");
        }
        
        public ActionResult Account()
        {
            var idCookie = HttpContext.Request.Cookies["id"];

            var viewModel = new AccountViewModel();
            

            if (idCookie?.Value != null)
            {
                var user = GetDatabase().Get<User>(idCookie.Value);
                viewModel.Name = user?.Name;
            }

            return View(viewModel);
        }

        public ActionResult SetAccountDetails(AccountViewModel viewModel)
        {
            var idCookie = HttpContext.Request.Cookies["id"];
            User user;
            if (idCookie?.Value != null)
            {
                user = GetDatabase().Get<User>(idCookie.Value);
            }
            else
            {
                user = new User();
            }

            user.Name = viewModel.Name;

            GetDatabase().Save(user);

            idCookie = idCookie ?? new HttpCookie("id");

            idCookie.Value = user.Id;

            HttpContext.Response.Cookies.Set(idCookie);

            return RedirectToAction("account");
        }
    }

    public class AccountViewModel
    {
        public string Name { get; set; }
    }

    public class User : IPersistable
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}