using intrapp.DataAccess.RiotGamesApi;
using intrapp.Extensions.String;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace intrapp.Models.Utils
{
    public static class SummonerInfoUtils
    {
        private static List<QueueType> QueueTypes { get; set; } = GetQueueTypes();
        private static List<RunePath> RunePaths { get; set; } = GetRunePaths();
        private static List<SummonerSpell> SummonerSpells { get; set; } = GetSummonerSpells();

        /// <summary>
        /// Populates the custom fields of a participant for display on the Summoner Info view.
        /// </summary>
        /// <param name="participant"></param>
        /// <param name="match"></param>
        /// <param name="jsonData"></param>
        public static void SetParticipantCustomFieldsAndDeltas(Participant participant, Match match, string jsonData)
        {
            var pathBuilder = new UrlPathBuilder();
            var temp = RunePaths;
            participant.Player = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId).Player;
            participant.ChampionPlayedIcon = pathBuilder.GetChampionIconUrl(participant.ChampionId);
            SetTimeLineStatsOfParticipant(participant, match, jsonData);

            var displayedSummonerName = participant.Player.SummonerName.Truncate(150);
            participant.DisplayedSummonerName = displayedSummonerName == participant.Player.SummonerName ? displayedSummonerName : displayedSummonerName + "...";
        }

        /// <summary>
        /// Populates the custom fields of a match for display on the Summoner Info view.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="matchRef"></param>
        /// <param name="accountId"></param>
        public static void SetMatchCustomFields(Match match, MatchReference matchRef, string accountId)
        {
            var pathBuilder = new UrlPathBuilder();
            var participantIdentity = match.ParticipantIdentities.FirstOrDefault(pi => pi.Player.AccountId == accountId);
            var participant = match.Participants.FirstOrDefault(p => p.ParticipantId == participantIdentity.ParticipantId);

            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);
            match.Timestamp = matchRef.Timestamp;
            match.WasPlayed = GetMatchWasPlayedTime(match.Timestamp);
            match.GameDurationStr = GetGameDurationInText(match.GameDuration);
            match.QueueTypeName = GetMatchQueueTypeName(match.QueueId);
            match.GameResult = participant.Stats.Win == true ? "Victory" : "Defeat";

            var spell1Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell1Id).IconPath;
            var spell2Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell2Id).IconPath;

            var perkStyle = pathBuilder.GetRuneIcon(RunePaths.FirstOrDefault(rp => rp.Id == participant.Stats.PerkSubStyle).Icon);
            var keystonePath = "";
            foreach (var path in RunePaths)
                foreach (var slot in path.Slots)
                    foreach (var rune in slot.Runes)
                        if (rune.Id == participant.Stats.Perk0)
                            keystonePath = pathBuilder.GetRuneIcon(rune.Icon);

            match.ChampionForDisplay = new ChampionForDisplay()
            {
                ChampionIconUrl = pathBuilder.GetChampionIconUrl(participant.ChampionId),
                SummonerSpell1IconUrl = pathBuilder.GetSummonerSpellIcon(spell1Path.Replace("/lol-game-data/assets/", "").ToLower()),
                SummonerSpell2IconUrl = pathBuilder.GetSummonerSpellIcon(spell2Path.Replace("/lol-game-data/assets/", "").ToLower()),
                RuneKeystoneIconUrl = keystonePath,
                RuneSecondaryPathIconUrl = perkStyle
            };
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

        private static string GetGameDurationInText(long gameDuration)
        {
            TimeSpan time = TimeSpan.FromSeconds(gameDuration);

            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }
    }
}