using Newtonsoft.Json;
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
            return DataDragonUrlPaths.DDRAGON_BASE_CDN + ddragon_versions.First() + "/";
        }

        public string GetSummonerByNameUrl(string name)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == BaseUrlPaths.DEFAULT_PLATFORM);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_SUMMONER_BY_NAME + name;
        }

        public string GetMatchListOfSummonerByAccountIdUrl(string accountId)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == BaseUrlPaths.DEFAULT_PLATFORM);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_MATCH_HISTORY_BY_ACCOUNTID + accountId + ApiUrlPaths.PARAMETER_DEFAULT_ENDINDEX;
        }

        public string GetMatchListOfSummonerByAccountIdUrl(string accountId, int startIndex, int endIndex)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == BaseUrlPaths.DEFAULT_PLATFORM);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_MATCH_HISTORY_BY_ACCOUNTID + accountId + "?endIndex=" + endIndex + "&beginIndex=" + startIndex;
        }

        public string GetMatchByGameIdUrl(long gameId)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == BaseUrlPaths.DEFAULT_PLATFORM);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_MATCH_BY_GAMEID + gameId.ToString();
        }

        public string GetLeagueEntriesBySummonerIdUrl(string summonerId)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == BaseUrlPaths.DEFAULT_PLATFORM);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_LEAGUE_ENTRY_BY_SUMMONERID + summonerId;
        }

        public string GetProfileIconUrl(int profileIconId)
        {
            return GetDDragonCdn() + DataDragonUrlPaths.DDRAGON_PROFILEICON + profileIconId.ToString() + ".png";
        }
    }
}