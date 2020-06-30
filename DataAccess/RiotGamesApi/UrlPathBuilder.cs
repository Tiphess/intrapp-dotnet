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
            var ddragon_url_provider = new StaticDataPathProvider();
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
            var ddragon_url_provider = new StaticDataPathProvider();
            return ddragon_url_provider.DDRAGON_GET.Replace("{version}", DDragonLatestVersion);
        }

        public string GetSummonerByNameUrl(string name, string region)
        {
            var platform = PlatformProvider.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return platform.Value + ApiPathProvider.GET_SUMMONER_BY_NAME.Replace("{summonerName}", name);
        }

        public string GetMatchListOfSummonerByAccountIdUrl(string accountId, string region)
        {
            var platform = PlatformProvider.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return platform.Value + ApiPathProvider.GET_MATCH_HISTORY_BY_ACCOUNTID.Replace("{accountId}", accountId) + ApiPathProvider.DEFAULT_ENDINDEX;
        }

        public string GetMatchListOfSummonerByAccountIdUrl(string accountId, int startIndex, int endIndex, string region)
        {
            var platform = PlatformProvider.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return platform.Value + ApiPathProvider.GET_MATCH_HISTORY_BY_ACCOUNTID.Replace("{accountId}", accountId) + "?endIndex=" + endIndex + "&beginIndex=" + startIndex;
        }

        public string GetMatchByGameIdUrl(long gameId, string region)
        {
            var platform = PlatformProvider.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return platform.Value + ApiPathProvider.GET_MATCH_BY_GAMEID.Replace("{gameId}", gameId.ToString());
        }

        public string GetLeagueEntriesBySummonerIdUrl(string summonerId, string region)
        {
            var platform = PlatformProvider.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return platform.Value + ApiPathProvider.GET_LEAGUE_ENTRY_BY_SUMMONERID.Replace("{summonerId}", summonerId);
        }

        public string GetProfileIconUrl(int profileIconId)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_PROFILEICON.Replace("{profileIconId}", profileIconId.ToString());
        }

        public string GetChampionIconUrl(int championId)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return ddragon_url_provider.CDRAGON_GET_CHAMPION_ICON.Replace("{championId}", championId.ToString());
        }

        public string GetRunesReforgedUrl()
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_RUNES_DATA;
        }

        public string GetRuneIcon(string iconPath)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return ddragon_url_provider.DDRAGON_VERSIONLESS_IMG.Replace("{path}", iconPath);
        }

        public string GetSummonerSpellsUrl()
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return ddragon_url_provider.CDRAGON_SUMMONERSPELLS_JSON;
        }

        public string GetChampionsUrl()
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_CHAMPION_DATA;
        }

        public string GetSummonerSpellIcon(string summonerSpellFilePath)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return ddragon_url_provider.CDRAGON_PREFIX.Replace("{path}", summonerSpellFilePath);
        }

        public string GetItemIcon(int itemId)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            if (itemId == 0)
                return "../DataAccess/RiotGamesApi/Images/Misc/no-item.png";

            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_ITEM_ICON.Replace("{itemId}", itemId.ToString());
        }

        public string GetSpellIcon(string spellId)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            if (spellId == "0" || string.IsNullOrWhiteSpace(spellId))
                return "../DataAccess/RiotGamesApi/Images/Misc/no-item.png";

            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_CHAMPION_SPELL.Replace("{spellId}", spellId);
        }

        public string GetMatchTimelineUrl(long gameId, string region)
        {
            var platform = PlatformProvider.PLATFORMS.FirstOrDefault(p => p.Key == region);
            return platform.Value + ApiPathProvider.GET_MATCH_TIMELINE_BY_GAMEID.Replace("{matchId}", gameId.ToString());
        }

        public string GetSpecificChampionUrl(string championId)
        {
            var ddragon_url_provider = new StaticDataPathProvider();
            return GetDDragonVersion() + ddragon_url_provider.DDRAGON_SPECIFIC_CHAMPION_DATA.Replace("{championId}", championId);
        }
    }
}