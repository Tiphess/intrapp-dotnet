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
            summonerInfo.Region = region;
            summonerInfo.LastPlayed = GetLastTimePlayedStr(summonerInfo.MatchHistory);


            foreach (var leagueEntry in summonerInfo.LeagueEntries)
                leagueEntry.WinRate = (int)Math.Round((double) leagueEntry.Wins / (leagueEntry.Wins + leagueEntry.Losses) * 100);

            return summonerInfo;
        }

        public List<Match> FetchMoreMatches(string accountId, int startIndex, int endIndex, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = GetMatchListOfSummoner(accountId, startIndex, endIndex, region);
            var matches = new List<Match>();

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
                            {
                                participant.Player = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId).Player;
                                participant.ChampionPlayedIcon = pathBuilder.GetChampionIconUrl(participant.ChampionId);
                                SetTimeLineStatsOfParticipant(participant, match, readData.Result);

                                //kinda bad, will revisist
                                if (participant.Player.SummonerName.Length > 15)
                                    participant.DisplayedSummonerName = participant.Player.SummonerName.Substring(0, 12) + "..";
                            }

                            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);
                            match.Timestamp = matchRef.Timestamp;
                            matches.Add(match);
                        }
                    }
                }
                catch (Exception) {}
            }
            return matches;
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
                catch (Exception) {}
            }
            return summoner;
        }

        private MatchHistory GetMatchHistoryOfSummoner(string accountId, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = GetMatchListOfSummoner(accountId, region);
            var matchHistory = new MatchHistory()
            {
                StartIndex = 0,
                EndIndex = 5,
            };
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
                            {
                                participant.Player = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId).Player;
                                participant.ChampionPlayedIcon = pathBuilder.GetChampionIconUrl(participant.ChampionId);
                                SetTimeLineStatsOfParticipant(participant, match, readData.Result);

                                //kinda bad, will revisist
                                if (participant.Player.SummonerName.Length > 15)
                                    participant.DisplayedSummonerName = participant.Player.SummonerName.Substring(0, 12) + "..";
                            }

                            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);
                            match.Timestamp = matchRef.Timestamp;
                            matchHistory.Matches.Add(match);
                        }
                    }
                }
                catch (Exception) {}
            }
            return matchHistory;
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
            return leagueEntries;
        }

        private MatchList GetMatchListOfSummoner(string accountId, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = new MatchList();
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetMatchListOfSummonerByAccountIdUrl(accountId, region)));
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

        private MatchList GetMatchListOfSummoner(string accountId, int startIndex, int endIndex, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var matchList = new MatchList();
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetMatchListOfSummonerByAccountIdUrl(accountId, startIndex, endIndex, region)));
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

        private string GetLastTimePlayedStr(MatchHistory matchHistory)
        {
            var matchTimestmap = matchHistory.Matches.First().Timestamp;

            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastMatchTime = date.AddMilliseconds(matchTimestmap).ToLocalTime();
            var difference = (DateTime.Now - lastMatchTime).Hours;

            return "Played " + difference + " hours ago";
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