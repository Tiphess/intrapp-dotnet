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

        public ActionResult SummonerInfo(string summonerName)
        {
            var model = dllSummonerInfo.GetSummoner(summonerName);
            return View(model);
        }
    }
}