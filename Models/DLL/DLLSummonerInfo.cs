using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using intrapp.DataAccess;
using intrapp.DataAccess.RiotGamesApi;
using intrapp.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace intrapp.Models.DLL
{
    public class DLLSummonerInfo
    {

        public SummonerInfo GetSummoner(string name)
        {
            var pathBuilder = new UrlPathBuilder();
            var summonerInfo = new SummonerInfo();

            summonerInfo.Summoner = GetSummonerByName(name);
            summonerInfo.MatchHistory = GetMatchHistoryOfSummoner(summonerInfo.Summoner.AccountId);
            summonerInfo.LeagueEntries = GetLeagueEntriesOfSummoner(summonerInfo.Summoner.Id);
            summonerInfo.ProfileIconUrl = pathBuilder.GetProfileIconUrl(summonerInfo.Summoner.ProfileIconId);

            return summonerInfo;
        }

        public List<Match> FetchMoreMatches(string accountId, int startIndex, int endIndex)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = GetMatchListOfSummoner(accountId, startIndex, endIndex);
            var matches = new List<Match>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                foreach (var matchRef in matchList.Matches)
                {
                    var response = client.GetAsync(new Uri(pathBuilder.GetMatchByGameIdUrl(matchRef.GameId)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        var match = JsonConvert.DeserializeObject<Match>(readData.Result);
                        foreach (var participant in match.Participants)
                            SetTimeLineStatsOfParticipant(participant, match, readData.Result);

                        matches.Add(match);
                    }
                }
            }
            return matches;
        }

        private Summoner GetSummonerByName(string name)
        {
            var pathBuilder = new UrlPathBuilder();
            var summoner = new Summoner();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                var response = client.GetAsync(new Uri(pathBuilder.GetSummonerByNameUrl(name)));
                response.Wait();

                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readData = result.Content.ReadAsStringAsync();
                    readData.Wait();

                    summoner = JsonConvert.DeserializeObject<Summoner>(readData.Result);
                }
            }
            return summoner;
        }
        private MatchHistory GetMatchHistoryOfSummoner(string accountId)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = GetMatchListOfSummoner(accountId);
            var matchHistory = new MatchHistory()
            {
                StartIndex = 0,
                EndIndex = 5,
            };
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                foreach (var matchRef in matchList.Matches)
                {
                    var response = client.GetAsync(new Uri(pathBuilder.GetMatchByGameIdUrl(matchRef.GameId)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        var match = JsonConvert.DeserializeObject<Match>(readData.Result);
                        foreach (var participant in match.Participants)
                            SetTimeLineStatsOfParticipant(participant, match, readData.Result);

                        matchHistory.Matches.Add(match);
                    }
                }
            }
            return matchHistory;
        }

        private List<LeagueEntry> GetLeagueEntriesOfSummoner(string summonerId)
        {
            var pathBuilder = new UrlPathBuilder();
            var leagueEntries = new List<LeagueEntry>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                var response = client.GetAsync(new Uri(pathBuilder.GetLeagueEntriesBySummonerIdUrl(summonerId)));
                response.Wait();

                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readData = result.Content.ReadAsStringAsync();
                    readData.Wait();

                    leagueEntries = JsonConvert.DeserializeObject<List<LeagueEntry>>(readData.Result);
                }
            }

            return leagueEntries;
        }

        private MatchList GetMatchListOfSummoner(string accountId)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = new MatchList();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                var response = client.GetAsync(new Uri(pathBuilder.GetMatchListOfSummonerByAccountIdUrl(accountId)));
                response.Wait();

                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readData = result.Content.ReadAsStringAsync();
                    readData.Wait();

                    matchList = JsonConvert.DeserializeObject<MatchList>(readData.Result);
                }
            }
            return matchList;
        }

        private MatchList GetMatchListOfSummoner(string accountId, int startIndex, int endIndex)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = new MatchList();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                var response = client.GetAsync(new Uri(pathBuilder.GetMatchListOfSummonerByAccountIdUrl(accountId, startIndex, endIndex)));
                response.Wait();

                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readData = result.Content.ReadAsStringAsync();
                    readData.Wait();

                    matchList = JsonConvert.DeserializeObject<MatchList>(readData.Result);
                }
            }
            return matchList;
        }

        //Seems really ugly but it'll do for now
        private void SetTimeLineStatsOfParticipant(Participant participant, Match match, string jsonData)
        {
            var numberOfDeltasFields = match.GameDuration / 600;
            var jsonObject = JObject.Parse(jsonData);
            foreach (var part in jsonObject["participants"])
            {
                if (Convert.ToInt32(part["timeline"]["participantId"]) == participant.ParticipantId)
                {
                    var timeline = part.Children<JProperty>().FirstOrDefault(x => x.Name == "timeline");
                    var timelineProperties = (JObject)timeline.Value;

                    foreach (var prop in timelineProperties.Properties())
                    {
                        if (prop.Name == "participantId")
                            continue;

                        var deltas = prop.Value.Children<JProperty>();
                        switch (prop.Name)
                        {
                            case "creepsPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.CreepsPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                            case "xpPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.XpPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                            case "goldPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.GoldPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                            case "csDiffPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.CsDiffPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                            case "xpDiffPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.XpDiffPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                            case "damageTakenPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.DamageTakenPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                            case "damageTakenDiffPerMinDeltas":
                                foreach (var dProp in deltas.Reverse()) participant.Timeline.DamageTakenDiffPerMinDeltas.Data.Add(dProp.Name, Convert.ToDouble(dProp.Value));
                                break;
                        }
                    }
                    break;
                }
            }
        }
    }
}