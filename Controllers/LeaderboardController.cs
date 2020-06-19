using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace intrapp.Controllers
{
    public class LeaderboardController : Controller
    {
        // GET: Leaderboard
        public ActionResult Index()
        {
            SetRegions();
            return View();
        }

        public void SetRegions()
        {
            ViewBag.Regions = new Dictionary<string, string>()
            {
                { "BR1", "BR" },
                { "EUN1", "EUN" },
                { "EUW1", "EUW" },
                { "JP1", "JP" },
                { "KR", "KR" },
                { "LA1", "LAN" },
                { "LA2", "LAS" },
                { "NA1", "NA" },
                { "OC1", "OCE" },
                { "TR1", "TR" },
                { "RU", "RU" },
            };
        }
    }
}