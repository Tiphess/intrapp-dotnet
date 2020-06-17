using intrapp.DataAccess.RiotGamesApi;
using intrapp.Extensions.String;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models.Utils
{
    public static class SummonerInfoUtils
    {

        public static void SetLeagueEntriesWinRates(List<LeagueEntry> leagueEntries)
        {

        }
        /// <summary>
        /// Populates the custom fields of a participant for display on the Summoner Info view.
        /// </summary>
        /// <param name="participant"></param>
        /// <param name="match"></param>
        /// <param name="jsonData"></param>
        public static void SetParticipantCustomFieldsAndDeltas(Participant participant, Match match, string jsonData)
        {
            var pathBuilder = new UrlPathBuilder();

            participant.Player = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId).Player;
            participant.DisplayedSummonerName = participant.Player.SummonerName.Truncate(150);
            participant.ChampionPlayedIcon = pathBuilder.GetChampionIconUrl(participant.ChampionId);
            SetTimeLineStatsOfParticipant(participant, match, jsonData);
        }

        /// <summary>
        /// Returns the last time a Summoner has played a match.
        /// </summary>
        /// <param name="matchHistory"></param>
        /// <returns></returns>
        public static string GetLastTimePlayedStr(MatchHistory matchHistory)
        {
            //todo Returns "x minutes ago" or "x days ago" instead of only "x hours ago"
            var matchTimestmap = matchHistory.Matches.First().Timestamp;

            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastMatchTime = date.AddMilliseconds(matchTimestmap).ToLocalTime();
            var difference = (DateTime.Now - lastMatchTime).Hours;

            return "Played " + difference + " hours ago";
        }

        /// <summary>
        /// Populates the timeline's deltas of a participant in a custom dictionary.
        /// </summary>
        /// <param name="participant"></param>
        /// <param name="match"></param>
        /// <param name="jsonData"></param>
        //Seems really ugly but it'll do for now
        public static void SetTimeLineStatsOfParticipant(Participant participant, Match match, string jsonData)
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