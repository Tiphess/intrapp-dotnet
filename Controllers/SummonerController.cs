using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using intrapp.Models;
using System.Web.Configuration;
using intrapp.DataAccess.RiotGamesApi;
using intrapp.Models.DLL;
using intrapp.Models.ViewModels;

namespace intrapp.Controllers
{
    public class SummonerController : Controller
    {
        private DLLSummonerInfo dllSummonerInfo = new DLLSummonerInfo();

        public ActionResult SummonerInfo(string summonerName, string region)
        {
            var model = dllSummonerInfo.GetSummoner(summonerName, region);
            if (model == null)
            {
                var errorMessage = "Data not found - Please enter a summoner name.";
                if (!string.IsNullOrWhiteSpace(summonerName))
                    errorMessage = "Data not found - Summoner with the name '" + summonerName + "' in the region '" + region.Replace("1", "") + "' does not exist.";

                return RedirectToAction("Index", "Home", new { message = errorMessage, region = region });
            }
            
            SetRegions(region);
            return View(model);
        }

        public ActionResult _MatchHistory(string accountId, int startIndex, int endIndex, string region)
        {
            var model = dllSummonerInfo.FetchMoreMatches(accountId, region, startIndex, endIndex);
            ViewBag.SelectedRegion = region;
            return PartialView(model);
        }

        public ActionResult _MatchBreakdown(string accountId, string region, long gameId)
        {
            var model = dllSummonerInfo.GetMatch(gameId, region, accountId);
            ViewBag.SelectedRegion = region;
            return PartialView(model);
        }

        public void SetRegions(string region)
        {
            ViewBag.SelectedRegion = region;
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