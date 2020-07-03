using intrapp.Models;
using intrapp.Models.DLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;

namespace intrapp.Controllers
{
    public class LeaderboardController : Controller
    {
        private DLL_Leaderboards dll_leaderboards = new DLL_Leaderboards();

        // GET: Leaderboard
        public ActionResult Index()
        {
            SetRegions();
            SetDefaultValues();

            var model = dll_leaderboards.GetChallengers("RANKED_SOLO_5x5", "NA1"); // Default queue/region
            MemoryCache.Default["challengerLeague"] = new League(model);
            model.Entries = model.Entries.Take(League.PageSize).ToList(); // return page 1
            model.CurrentPage = 0;
            ViewBag.SetFirstPageActive = true;

            return View(model);
        }
        public ActionResult _ChallengerLeague(int page = 0, string queue = null, string region = null, int isPagination = 0)
        {
            if (page == 0) ViewBag.SetFirstPageActive = true;

            SetRegions();
            SetDefaultValues(region);
            League model;
            if (string.IsNullOrWhiteSpace(queue) && string.IsNullOrWhiteSpace(region))
            {
                model = dll_leaderboards.GetChallengers("RANKED_SOLO_5x5", "NA1"); // Default queue/region
                MemoryCache.Default["challengerLeague"] = new League(model);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }

            if (isPagination != 1)
            {
                model = dll_leaderboards.GetChallengers(queue, region);
                MemoryCache.Default["challengerLeague"] = new League(model);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }
            else
            {
                var tempModel = (League)MemoryCache.Default["challengerLeague"];
                model = new League(tempModel);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }
        }

        public ActionResult _GrandmasterLeague(int page = 0, string queue = null, string region = null, int isPagination = 0)
        {
            if (page == 0) ViewBag.SetFirstPageActive = true;

            SetRegions();
            SetDefaultValues(region);
            League model;
            if (string.IsNullOrWhiteSpace(queue) && string.IsNullOrWhiteSpace(region))
            {
                model = dll_leaderboards.GetGrandmasters("RANKED_SOLO_5x5", "NA1"); // Default queue/region
                MemoryCache.Default["grandmasterLeague"] = new League(model);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }

            if (isPagination != 1)
            {
                model = dll_leaderboards.GetGrandmasters(queue, region);
                MemoryCache.Default["grandmasterLeague"] = new League(model);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }
            else
            {
                var tempModel = (League)MemoryCache.Default["grandmasterLeague"];
                model = new League(tempModel);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }
        }

        public ActionResult _MasterLeague(int page = 0, string queue = null, string region = null, int isPagination = 0)
        {
            if (page == 0) ViewBag.SetFirstPageActive = true;

            SetRegions();
            SetDefaultValues(region);
            League model;
            if (string.IsNullOrWhiteSpace(queue) && string.IsNullOrWhiteSpace(region))
            {
                model = dll_leaderboards.GetMasters("RANKED_SOLO_5x5", "NA1"); // Default queue/region
                MemoryCache.Default["masterLeague"] = new League(model);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }

            if (isPagination != 1)
            {
                model = dll_leaderboards.GetMasters(queue, region);
                MemoryCache.Default["masterLeague"] = new League(model);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }
            else
            {
                var tempModel = (League) MemoryCache.Default["masterLeague"];
                model = new League(tempModel);
                dll_leaderboards.SetLeaguePages(page, model);
                return PartialView(model);
            }
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

        public void SetDefaultValues(string region = null)
        {
            ViewBag.SelectedRegion = string.IsNullOrWhiteSpace(region) ? "NA1" : region;
            ViewBag.Queues = new Dictionary<string, string>()
            {
                { "RANKED_SOLO_5x5", "Ranked Solo" },
                { "RANKED_FLEX_SR", "Ranked Flex" }
            };
            ViewBag.DefaultQueue = "RANKED_SOLO_5x5";
        }
    }
}