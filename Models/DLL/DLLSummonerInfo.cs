using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using intrapp.DataAccess;
using intrapp.DataAccess.RiotGamesApi;
using intrapp.Extensions.String;
using intrapp.Models.Utils;
using intrapp.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace intrapp.Models.DLL
{
    public class DLLSummonerInfo
    {
        public SummonerInfo GetSummoner(string name, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var summonerInfo = new SummonerInfo();

            summonerInfo.Summoner = GetSummonerByName(name, region);
            if (summonerInfo.Summoner.AccountId == null)
                return null;

            summonerInfo.MatchHistory = GetMatchHistoryOfSummoner(summonerInfo.Summoner.AccountId, region);
            summonerInfo.LeagueEntries = GetLeagueEntriesOfSummoner(summonerInfo.Summoner.Id, region);
            summonerInfo.ProfileIconUrl = pathBuilder.GetProfileIconUrl(summonerInfo.Summoner.ProfileIconId);
            summonerInfo.LastPlayed = SummonerInfoUtils.GetLastTimePlayedStr(summonerInfo.MatchHistory);
            summonerInfo.Region = region;

            return summonerInfo;
        }

        private Summoner GetSummonerByName(string name, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var summoner = new Summoner();

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetSummonerByNameUrl(name, region)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        summoner = JsonConvert.DeserializeObject<Summoner>(readData.Result);
                    }
                }
                catch (Exception) { }
            }
            return summoner;
        }

        public List<Match> FetchMoreMatches(string accountId, string region, int startIndex, int endIndex)
        {
           return GetMatchHistoryOfSummoner(accountId, region, startIndex, endIndex).Matches;
        }

        private MatchHistory GetMatchHistoryOfSummoner(string accountId, string region, int? startIndex = null, int? endIndex = null)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchHistory = new MatchHistory();
            var matchList = new MatchList();

            if (startIndex.HasValue && endIndex.HasValue)
                matchList = GetMatchListOfSummoner(accountId, region, startIndex.Value, endIndex.Value);
            else
                matchList = GetMatchListOfSummoner(accountId, region);

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    foreach (var matchRef in matchList.Matches)
                    {
                        var response = client.GetAsync(new Uri(pathBuilder.GetMatchByGameIdUrl(matchRef.GameId, region)));
                        response.Wait();

                        var result = response.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readData = result.Content.ReadAsStringAsync();
                            readData.Wait();

                            var match = JsonConvert.DeserializeObject<Match>(readData.Result);
                            foreach (var participant in match.Participants)
                                SummonerInfoUtils.SetParticipantCustomFieldsAndDeltas(participant, match, readData.Result);

                            SummonerInfoUtils.SetMatchCustomFields(match, accountId, matchRef);
                            matchHistory.Matches.Add(match);
                        }
                    }
                }
                catch (Exception) {}
            }
            return matchHistory;
        }

        public MatchBreakdown GetMatch(long gameId, string region, string accountId)
        {
            var pathBuilder = new UrlPathBuilder();
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetMatchByGameIdUrl(gameId, region)));
                    response.Wait();

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var readData = response.Result.Content.ReadAsStringAsync();
                        readData.Wait();

                        var match = JsonConvert.DeserializeObject<MatchBreakdown>(readData.Result);
                        SummonerInfoUtils.SetMatchBreakdownFields(match);

                        return match;
                    }
                }
                catch (Exception) { return new MatchBreakdown(); }
            }
            return new MatchBreakdown();
        }

        private List<LeagueEntry> GetLeagueEntriesOfSummoner(string summonerId, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var leagueEntries = new List<LeagueEntry>();

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetLeagueEntriesBySummonerIdUrl(summonerId, region)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        leagueEntries = JsonConvert.DeserializeObject<List<LeagueEntry>>(readData.Result);
                    }
                }
                catch (Exception) {}
            }

            SummonerInfoUtils.SetLeagueEntriesWinRates(leagueEntries);
            return leagueEntries;
        }

        private MatchList GetMatchListOfSummoner(string accountId, string region, int? startIndex = null, int? endIndex = null)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = new MatchList();
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    Task<HttpResponseMessage> response;
                    if (startIndex.HasValue && endIndex.HasValue)
                        response = client.GetAsync(new Uri(pathBuilder.GetMatchListOfSummonerByAccountIdUrl(accountId, startIndex.Value, endIndex.Value, region)));
                    else
                        response = client.GetAsync(new Uri(pathBuilder.GetMatchListOfSummonerByAccountIdUrl(accountId, region)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        matchList = JsonConvert.DeserializeObject<MatchList>(readData.Result);
                    }
                }
                catch (Exception) {}
            }
            return matchList;
        }
    }
}