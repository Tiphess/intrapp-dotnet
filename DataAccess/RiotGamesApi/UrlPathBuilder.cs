using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using intrapp.DataAccess.RiotGamesApi;
using System.Runtime.Remoting.Messaging;
using intrapp.Models;

namespace intrapp.DataAccess.RiotGamesApi
{
    public class UrlPathBuilder
    {
        private static string DDragonLatestVersion { get; set; } = SetDDragonVersion();

        public static string SetDDragonVersion()
        {
            var ddragon_versions = new List<string>();
            var ddragon_url_provider = new DataDragonUrlPaths();
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(new Uri(ddragon_url_provider.GET_DDRAGON_VERSIONS));
                response.Wait();
                if (response.Result.IsSuccessStatusCode)
                {
                    var readData = response.Result.Content.ReadAsStringAsync();
                    readData.Wait();
                    ddragon_versions = JsonConvert.DeserializeObject<List<string>>(readData.Result);
                }
            }
            return ddragon_versions.First();
        }

        public string GetDDragonVersion()
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return ddragon_url_provider.DDRAGON_BASE_CDN + DDragonLatestVersion + "/";
        }

        public string GetSummonerByNameUrl(string name, string region)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_SUMMONER_BY_NAME + name;
        }

        public string GetMatchListOfSummonerByAccountIdUrl(string accountId, string region)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_MATCH_HISTORY_BY_ACCOUNTID + accountId + ApiUrlPaths.PARAMETER_DEFAULT_ENDINDEX;
        }

        public string GetMatchListOfSummonerByAccountIdUrl(string accountId, int startIndex, int endIndex, string region)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_MATCH_HISTORY_BY_ACCOUNTID + accountId + "?endIndex=" + endIndex + "&beginIndex=" + startIndex;
        }

        public string GetMatchByGameIdUrl(long gameId, string region)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_MATCH_BY_GAMEID + gameId.ToString();
        }

        public string GetLeagueEntriesBySummonerIdUrl(string summonerId, string region)
        {
            var platform = BaseUrlPaths.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return BaseUrlPaths.HTTPS + platform.Value + ApiUrlPaths.GET_LEAGUE_ENTRY_BY_SUMMONERID + summonerId;
        }

        public string GetProfileIconUrl(int profileIconId)
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_PROFILEICON + profileIconId.ToString() + ".png";
        }

        public string GetChampionIconUrl(int championId)
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return ddragon_url_provider.CDRAGON_BASE + DDragonLatestVersion + ddragon_url_provider.CDRAGON_CHAMPION + championId + ddragon_url_provider.CDRAGON_ICON;
        }

        public string GetRunesReforgedUrl()
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_RUNES_JSON;
        }

        public string GetRuneIcon(string iconPath)
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return ddragon_url_provider.DDRAGON_VERSIONLESS_IMG + iconPath;
        }

        public string GetSummonerSpellsUrl()
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return ddragon_url_provider.CDRAGON_SUMMONERSPELLS_JSON;
        }

        public string GetChampionsUrl()
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_CHAMPION_DATA;
        }

        public string GetSummonerSpellIcon(string summonerSpellFilePath)
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            return ddragon_url_provider.CDRAGON_PREFIX +  summonerSpellFilePath;
        }

        public string GetItemIcon(int itemId)
        {
            var ddragon_url_provider = new DataDragonUrlPaths();
            if (itemId == 0)
                return "../DataAccess/RiotGamesApi/Images/Misc/no-item.png";

            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_ITEM_ICON.Replace("{itemId}", itemId.ToString());
        }
    }
}