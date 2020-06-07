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

namespace intrapp.Controllers
{
    public class SummonerController : Controller
    {

        private string urlSummoner = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/";

        public async Task<string> GetSummonerStr()
        {
            var result = await GetSummoner();

            return result;
        }

        private async Task<string> GetSummoner()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(urlSummoner);
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public ActionResult Index()
        {
            var summoner_info = new Summoner();
            summoner_info.Json = "Empty";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlSummoner);
                var responseTask = client.GetAsync("incapable68?api_key=" + WebConfigurationManager.AppSettings["RIOT_GAMES_API_DEV_KEY"]);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    summoner_info.Json = readTask.Result;
                }
            }

            return View(summoner_info);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}