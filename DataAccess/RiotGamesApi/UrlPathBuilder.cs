﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using intrapp.DataAccess.RiotGamesApi;

namespace intrapp.DataAccess.RiotGamesApi
{
    public class UrlPathBuilder
    {
        public string GetDDragonCdn()
        {
            var ddragon_versions = new List<string>();
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(new Uri(DataDragonUrlPaths.GET_DDRAGON_VERSIONS));
                response.Wait();

                if (response.Result.IsSuccessStatusCode)
                {
                    var readData = response.Result.Content.ReadAsStringAsync();
                    readData.Wait();

                    ddragon_versions = JsonConvert.DeserializeObject<List<string>>(readData.Result);
                }
            }
            return DataDragonUrlPaths.GET_DDRAGON_VERSIONS + ddragon_versions.First() + "/";
        }

        public string GetSummonerByNameUrl(string name)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == BaseUrlPaths.DEFAULT_PLATFORM);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_SUMMONER_BY_NAME + name + ApiUrlPaths.API_KEY_QUERY_PARAMETER + ConfigWrapper.ApiKey;
        }
    }
}