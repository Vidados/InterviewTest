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
    public class ConfigController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string sequence)
        {
            var db = GetDatabase();

            // remove all past configurations
            db.DeleteAll<Config>();

            // add new configuration
            db.Save(new Config() { Sequence = sequence });

            // update UI
            TempData["notification"] = "Configuration saved";

            return RedirectToAction("index");
        }

        public class ConfigViewModel
        {
            public string sequence { get; set; }
        }

        private FileSystemDatabase GetDatabase() => new FileSystemDatabase();
    }
}