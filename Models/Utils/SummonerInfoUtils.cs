using intrapp.DataAccess;
using intrapp.DataAccess.RiotGamesApi;
using intrapp.Extensions.String;
using intrapp.Models.DLL;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace intrapp.Models.Utils
{
    public static class SummonerInfoUtils
    {
        private static List<QueueType> QueueTypes { get; set; } = GetQueueTypes();
        private static List<RunePath> RunePaths { get; set; } = GetRunePaths();
        private static List<SummonerSpell> SummonerSpells { get; set; } = GetSummonerSpells();
        private static List<Champion> Champions { get; set; } = GetChampions();

        //Used to determine the average rank per match
        

        /// <summary>
        /// Populates the custom properties of a participant for display on the Summoner Info view.
        /// </summary>
        /// <param name="participant"></param>
        /// <param name="match"></param>
        /// <param name="jsonData"></param>
        public static void SetParticipantCustomFieldsAndDeltas(Participant participant, Match match, string jsonData)
        {
            var pathBuilder = new UrlPathBuilder();
            participant.Player = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId).Player;
            participant.ChampionPlayedIcon = pathBuilder.GetChampionIconUrl(participant.ChampionId);
            SetTimeLineStatsOfParticipant(participant, match, jsonData);

            var displayedSummonerName = participant.Player.SummonerName.Truncate(150);
            participant.DisplayedSummonerName = displayedSummonerName == participant.Player.SummonerName ? displayedSummonerName : displayedSummonerName + "...";
        }

        /// <summary>
        /// Populates the custom properties of a match for display on the Summoner Info view.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="matchRef"></param>
        /// <param name="accountId"></param>
        public static void SetMatchCustomFields(Match match, string accountId, MatchReference matchRef = null)
        {
            var pathBuilder = new UrlPathBuilder();
            var participantIdentity = match.ParticipantIdentities.FirstOrDefault(pi => pi.Player.AccountId == accountId);
            var participant = match.Participants.FirstOrDefault(p => p.ParticipantId == participantIdentity.ParticipantId);
            //Custom properties for display
            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);
            match.Timestamp = matchRef.Timestamp;
            match.WasPlayed = GetMatchWasPlayedTime(match.Timestamp);
            match.GameDurationStr = GetGameDurationInText(match.GameDuration);
            match.QueueTypeName = GetMatchQueueTypeName(match.QueueId);
            match.GameResult = participant.Stats.Win == true ? "Victory" : "Defeat";
            //match.TierAverage = GetTierAverage(match); Costs too much resource!

            //Summoner spells icons
            var spell1Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell1Id).IconPath;
            var spell2Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell2Id).IconPath;

            //Runes icons
            var perkStyle = pathBuilder.GetRuneIcon(RunePaths.FirstOrDefault(rp => rp.Id == participant.Stats.PerkSubStyle).Icon);
            var keystonePath = "";
            foreach (var path in RunePaths)
                foreach (var slot in path.Slots)
                    foreach (var rune in slot.Runes)
                        if (rune.Id == participant.Stats.Perk0)
                            keystonePath = pathBuilder.GetRuneIcon(rune.Icon);

            //KillParticipation property
            var team = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == participant.TeamId);
            var totalTeamKills = 0;
            foreach (var player in team)
                totalTeamKills += player.Stats.Kills;

            var kp = (int)Math.Round((double)(participant.Stats.Kills + participant.Stats.Assists) / totalTeamKills * 100);
            match.ParticipantForDisplay = new ParticipantForDisplay()
            {
                ChampionIconUrl = pathBuilder.GetChampionIconUrl(participant.ChampionId),
                SummonerSpell1IconUrl = pathBuilder.GetSummonerSpellIcon(spell1Path.Replace("/lol-game-data/assets/", "").ToLower()),
                SummonerSpell2IconUrl = pathBuilder.GetSummonerSpellIcon(spell2Path.Replace("/lol-game-data/assets/", "").ToLower()),
                RuneKeystoneIconUrl = keystonePath,
                RuneSecondaryPathIconUrl = perkStyle,
                KillParticipationPercentage = kp,
                Items = GetItems(participant),
                ChampionName = Champions.FirstOrDefault(x => x.Key == participant.ChampionId.ToString()).Name,
                Participant = participant,
                ParticipantIdentity = participantIdentity
            };
        }

        public static void SetMatchBreakdownFields(MatchBreakdown match)
        {
            var pathBuilder = new UrlPathBuilder();
            var dll = new DLLSummonerInfo();

            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);

            var maxChampionKills = 0;
            var maxGoldEarned = 0;
            var maxDmgDealt = 0;
            var maxWardsPlaced = 0;
            var maxDmgTaken = 0;
            var maxCS = 0;
            foreach (var participant in match.Participants)
            {
                //Setting max values for analysis tab
                if (participant.Stats.Kills > maxChampionKills)
                    maxChampionKills = participant.Stats.Kills;
                if (participant.Stats.GoldEarned > maxGoldEarned)
                    maxGoldEarned = participant.Stats.GoldEarned;
                if (participant.Stats.TotalDamageDealtToChampions > maxDmgDealt)
                    maxDmgDealt = participant.Stats.TotalDamageDealtToChampions;
                if (participant.Stats.WardsPlaced > maxWardsPlaced)
                    maxWardsPlaced = participant.Stats.WardsPlaced;
                if (participant.Stats.TotalDamageTaken > maxDmgTaken)
                    maxDmgTaken = participant.Stats.TotalDamageTaken;
                if (participant.Stats.TotalMinionsKilled > maxCS)
                    maxCS = participant.Stats.TotalMinionsKilled;

                var participantIdentity = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId);

                //Summoner spells icons
                var spell1Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell1Id).IconPath;
                var spell2Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell2Id).IconPath;

                //Runes icons
                var perkStyle = pathBuilder.GetRuneIcon(RunePaths.FirstOrDefault(rp => rp.Id == participant.Stats.PerkSubStyle).Icon);
                var keystonePath = "";
                foreach (var path in RunePaths)
                    foreach (var slot in path.Slots)
                        foreach (var rune in slot.Runes)
                            if (rune.Id == participant.Stats.Perk0)
                                keystonePath = pathBuilder.GetRuneIcon(rune.Icon);

                //KillParticipation property
                var team = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == participant.TeamId);
                var totalTeamKills = 0;
                foreach (var player in team)
                    totalTeamKills += player.Stats.Kills;
                var kp = (int)Math.Round((double)(participant.Stats.Kills + participant.Stats.Assists) / totalTeamKills * 100);

                match.ParticipantsForDisplay.Add(new ParticipantForDisplay
                {
                    ChampionIconUrl = pathBuilder.GetChampionIconUrl(participant.ChampionId),
                    SummonerSpell1IconUrl = pathBuilder.GetSummonerSpellIcon(spell1Path.Replace("/lol-game-data/assets/", "").ToLower()),
                    SummonerSpell2IconUrl = pathBuilder.GetSummonerSpellIcon(spell2Path.Replace("/lol-game-data/assets/", "").ToLower()),
                    RuneKeystoneIconUrl = keystonePath,
                    RuneSecondaryPathIconUrl = perkStyle,
                    KillParticipationPercentage = kp,
                    Items = GetItems(participant),
                    ChampionName = Champions.FirstOrDefault(x => x.Key == participant.ChampionId.ToString()).Name,
                    Participant = participant,
                    ParticipantIdentity = participantIdentity,
                });
            }

            var teamsBreakdown = new TeamsBreakdown();
            foreach (var team in match.Teams)
            {
                if (team.TeamId == 100)
                {
                    teamsBreakdown.BlueTeamBaronKills = team.BaronKills;
                    teamsBreakdown.BlueTeamDragonKills = team.DragonKills;
                    teamsBreakdown.BlueTeamTowerKills = team.TowerKills;
                    teamsBreakdown.BlueTeamChampionKills = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == 100).Select(p => p.Stats.Kills).ToList().Sum();
                    teamsBreakdown.BlueTeamGold = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == 100).Select(p => p.Stats.GoldEarned).ToList().Sum();
                }
                else
                {
                    teamsBreakdown.RedTeamBaronKills = team.BaronKills;
                    teamsBreakdown.RedTeamDragonKills = team.DragonKills;
                    teamsBreakdown.RedTeamTowerKills = team.TowerKills;
                    teamsBreakdown.RedTeamChampionKills = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == 200).Select(p => p.Stats.Kills).ToList().Sum();
                    teamsBreakdown.RedTeamGold = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == 200).Select(p => p.Stats.GoldEarned).ToList().Sum();
                }
            }

            match.TeamsBreakdown = teamsBreakdown;
            match.HighestChampionKillsByAParticipant = maxChampionKills;
            match.HighestGoldEarnedAmountByAParticipant = maxGoldEarned;
            match.HighestDamageDealtToChampionsByAParticipant = maxDmgDealt;
            match.HighestWardsPlacedByAParticipant = maxWardsPlaced;
            match.HighestDamageTakenByAParticipant = maxDmgTaken;
            match.HighestCreepScoreByAParticipant = maxCS;
            match.ParticipantsForDisplayByTeam = match.ParticipantsForDisplay.GroupBy(p => p.Participant.TeamId);
        }

        public static void SetLeagueEntriesWinRates(List<LeagueEntry> leagueEntries)
        {
            foreach (var entry in leagueEntries)
                entry.WinRate = (int)Math.Round((double)entry.Wins / (entry.Wins + entry.Losses) * 100);
        }

        public static string GetLastTimePlayedStr(MatchHistory matchHistory)
        {
            //todo Returns "x minutes ago" or "x days ago" instead of only "x hours ago"
            var matchTimestmap = matchHistory.Matches.First().Timestamp;

            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastMatchTime = date.AddMilliseconds(matchTimestmap).ToLocalTime();
            var difference = (DateTime.Now - lastMatchTime).Hours;

            return "Played " + difference + " hours ago";
        }

        private static string GetMatchWasPlayedTime(long timestamp)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastMatchTime = date.AddMilliseconds(timestamp).ToLocalTime();
            var difference = (DateTime.Now - lastMatchTime).Hours;

            return difference + " hours ago";
        }

        private static List<Champion> GetChampions()
        {
            var pathBuilder = new UrlPathBuilder();
            var championList = new List<Champion>();
            using (var client = new WebClient())
            {
                try
                {
                    var championsJson = client.DownloadString(pathBuilder.GetChampionsUrl());
                    var jsonObject = JObject.Parse(championsJson);
                    foreach (JProperty champion in jsonObject["data"])
                    {
                        var championProperties = champion.Value;
                        var championName = championProperties["name"].ToString();
                        championList.Add(new Champion
                        {
                            Name = championProperties["name"].Value<string>(),
                            Key = championProperties["key"].Value<string>()
                        });
                    }

                    return championList;
                }
                catch (Exception) { return new List<Champion>(); }
            }
        }

        //Seems really ugly but it'll do for now
        private static void SetTimeLineStatsOfParticipant(Participant participant, Match match, string jsonData)
        {
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

        private static Inventory GetItems(Participant participant)
        {
            var pathBuilder = new UrlPathBuilder();
            return new Inventory()
            {
                Item0Url = pathBuilder.GetItemIcon(participant.Stats.Item0),
                Item1Url = pathBuilder.GetItemIcon(participant.Stats.Item1),
                Item2Url = pathBuilder.GetItemIcon(participant.Stats.Item2),
                Item3Url = pathBuilder.GetItemIcon(participant.Stats.Item3),
                Item4Url = pathBuilder.GetItemIcon(participant.Stats.Item4),
                Item5Url = pathBuilder.GetItemIcon(participant.Stats.Item5),
                Item6Url = pathBuilder.GetItemIcon(participant.Stats.Item6),
            };
        }

        private static string GetMatchQueueTypeName(int queueId)
        {
            var desc = QueueTypes.FirstOrDefault(qt => qt.QueueId == queueId).Description;
            //temp
            if (desc.Contains("Blind"))
                return "Blind";
            else if (desc.Contains("Draft"))
                return "Draft";
            else if (desc.Contains("Ranked Solo"))
                return "Ranked Solo";
            else if (desc.Contains("Ranked Flex"))
                return "Ranked Flex";

            return "Default";
        }

        private static string GetGameDurationInText(long gameDuration)
        {
            TimeSpan time = TimeSpan.FromSeconds(gameDuration);
            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }

        private static List<QueueType> GetQueueTypes()
        {
            using (var sr = new StreamReader(HttpRuntime.AppDomainAppPath + @"/DataAccess/RiotGamesApi/GameConstants/queues.json"))
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<QueueType>>(sr.ReadToEnd());
                }
                catch (Exception) { return new List<QueueType>(); }
            }
        }

        private static List<RunePath> GetRunePaths()
        {
            var pathBuilder = new UrlPathBuilder();
            using (var client = new WebClient())
            {
                try
                {
                    var runesJson = client.DownloadString(pathBuilder.GetRunesReforgedUrl());
                    return JsonConvert.DeserializeObject<List<RunePath>>(runesJson);
                }
                catch (Exception) { return new List<RunePath>(); }
            }
        }

        private static List<SummonerSpell> GetSummonerSpells()
        {
            var pathBuilder = new UrlPathBuilder();
            using (var client = new WebClient())
            {
                try
                {
                    var summonerSpellsJson = client.DownloadString(pathBuilder.GetSummonerSpellsUrl());
                    return JsonConvert.DeserializeObject<List<SummonerSpell>>(summonerSpellsJson);
                }
                catch (Exception) { return new List<SummonerSpell>(); }
            }
        }

        

        /* Data used for the GetTierAverage method
         * private readonly static Dictionary<int, string> Ranks = new Dictionary<int, string>
            {
                { 0, "IRON IV"},
                { 1, "IRON III"},
                { 2, "IRON II"},
                { 3, "IRON I"},
                { 4, "BRONZE IV"},
                { 5, "BRONZE III"},
                { 6, "BRONZE II"},
                { 7, "BRONZE I"},
                { 8, "SILVER IV"},
                { 9, "SILVER III"},
                { 10, "SILVER II"},
                { 11, "SILVER I"},
                { 12, "GOLD IV"},
                { 13, "GOLD III"},
                { 14, "GOLD II"},
                { 15, "GOLD I"},
                { 16, "PLATINUM IV"},
                { 17, "PLATINUM III"},
                { 18, "PLATINUM II"},
                { 19, "PLATINUM I"},
                { 20, "DIAMOND IV"},
                { 21, "DIAMOND III"},
                { 22, "DIAMOND II"},
                { 23, "DIAMOND I"},
                { 24, "MASTER I"},
                { 25, "GRANDMASTER I"},
                { 26, "CHALLENGER I"}
            };

            private readonly static Dictionary<string, int> RomanArabicRanks = new Dictionary<string, int>
            {
                { "I", 1},
                { "II", 2},
                { "III", 3},
                { "IV", 4},
            };
        */

        /*
         * Costs too much resource, so leaving it out for now
        private static string GetTierAverage(Match match)
        {
            var tierAverage = "";
            var rankValuesList = new List<int>();
            foreach (var participant in match.ParticipantIdentities)
            {
                var participantEntry = GetRank(participant.Player.SummonerId, participant.Player.CurrentPlatformId, match.QueueTypeName.ToLower());
                if (participantEntry != null)
                {
                    var rank = participantEntry.Tier + " " + participantEntry.Rank;
                    rankValuesList.Add(Ranks.FirstOrDefault(r => r.Value == rank).Key);
                }
                else
                    rankValuesList.Add(0);
            }

            var avg = rankValuesList.Average();
            var key = (int)Math.Round(avg, MidpointRounding.AwayFromZero);

            tierAverage = Ranks.FirstOrDefault(r => r.Key == key).Value;
            var splitTier = tierAverage.Split(' ');

            if (splitTier[1] == "IV")
                splitTier[1] = "4";
            else if (splitTier[1] == "III")
                splitTier[1] = "3";
            else if (splitTier[1] == "II")
                splitTier[1] = "2";
            else
                splitTier[1] = "1";

            return string.Join(" ", splitTier);
        }*/

        /*
        private static LeagueEntry GetRank(string summonerId, string region, string queueTypeName)
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

                        foreach (var entry in leagueEntries)
                            if (entry.QueueType.ToLower().Replace("_", " ").Contains(queueTypeName))
                                return entry;
                    }
                }
                catch (Exception) { return null;  }
            }
            return null;
        }*/
    }
}