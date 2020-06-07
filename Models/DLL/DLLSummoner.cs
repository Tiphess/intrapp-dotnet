using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using intrapp.DataAccess.RiotGamesApi;

namespace intrapp.Models.DLL
{
    public class DLLSummoner
    {
        public Summoner getSummonerByName(string name)
        {
            var pathBuilder = new UrlPathBuilder();

            var summoner_info = new Summoner();
            summoner_info.Json = "Empty";

            using (var client = new HttpClient())
            {
                var temp = pathBuilder.GetSummonerByNameUrl("incapable68");
                var responseTask = client.GetAsync(new Uri(pathBuilder.GetSummonerByNameUrl("incapable68")));
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    summoner_info.Json = readTask.Result;
                }
            }
            return summoner_info;
        }
    }
}