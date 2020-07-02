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

        #region Participant/Match Custom Fields setters
        public static void SetParticipantCustomFields(Participant participant, Match match, string jsonData)
        {
            var pathBuilder = new UrlPathBuilder();
            participant.Player = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId).Player;
            participant.ChampionPlayedIcon = pathBuilder.GetChampionIconUrl(participant.ChampionId);

            var displayedSummonerName = participant.Player.SummonerName.Truncate(150);
            participant.DisplayedSummonerName = displayedSummonerName == participant.Player.SummonerName ? displayedSummonerName : displayedSummonerName + "...";
        }

        public static void SetMatchCustomFields(Match match, string accountId, MatchReference matchRef = null)
        {
            var participantIdentity = match.ParticipantIdentities.FirstOrDefault(pi => pi.Player.AccountId == accountId);
            var participant = match.Participants.FirstOrDefault(p => p.ParticipantId == participantIdentity.ParticipantId);

            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);
            match.Timestamp = matchRef.Timestamp;
            match.WasPlayed = GetMatchWasPlayedTime(match.Timestamp);
            match.GameDurationStr = GetGameDurationInText(match.GameDuration);
            match.QueueTypeName = GetMatchQueueTypeName(match.QueueId);
            match.GameResult = participant.Stats.Win == true ? "Victory" : "Defeat";
            match.ParticipantForDisplay = SetParticipantForDisplay(match, participant, participantIdentity);
        }

        #endregion

        #region Match Breakdown Utils
        public static MatchBreakdown GetMatchBreakdown(Match match, string region, string accountId)
        {
            var pathBuilder = new UrlPathBuilder();
            var dll = new DLLSummonerInfo();
            var breakdown = new MatchBreakdown();

            var partIdentity = match.ParticipantIdentities.FirstOrDefault(pi => pi.Player.AccountId == accountId);
            var part = match.Participants.FirstOrDefault(p => p.ParticipantId == partIdentity.ParticipantId);

            match.ParticipantsByTeam = match.Participants.GroupBy(p => p.TeamId);
            match.ParticipantForDisplay = SetParticipantForDisplay(match, part, partIdentity);

            foreach (var participant in match.Participants)
            {
                var participantIdentity = match.ParticipantIdentities.FirstOrDefault(pi => pi.ParticipantId == participant.ParticipantId);
                breakdown.ParticipantsForDisplay.Add(SetParticipantForDisplay(match, participant, participantIdentity));
            }

            breakdown.Match = match;
            breakdown.TeamsBreakdown = SetTeamsBreakdown(match);
            breakdown.HighestKills = match.Participants.Max(p => p.Stats.Kills);
            breakdown.HighestGold = match.Participants.Max(p => p.Stats.GoldEarned);
            breakdown.HighestDmgDealt = match.Participants.Max(p => p.Stats.TotalDamageDealtToChampions);
            breakdown.HighestWardsPlaced = match.Participants.Max(p => p.Stats.WardsPlaced);
            breakdown.HighestDmgTaken = match.Participants.Max(p => p.Stats.TotalDamageTaken);
            breakdown.HighestCS = match.Participants.Max(p => p.Stats.TotalMinionsKilled);
            breakdown.ParticipantsForDisplayByTeam = breakdown.ParticipantsForDisplay.GroupBy(p => p.Participant.TeamId);
            breakdown.Timeline = GetMatchTimeline(match.GameId, region, partIdentity.ParticipantId);
            breakdown.Runes = GetRunesOfPlayer(part);
            breakdown.ObservedParticipant = part;
            breakdown.Spells = GetChampionAbilities(Champions.FirstOrDefault(c => c.Key == part.ChampionId.ToString()).Id);

            return breakdown;
        }

        private static TeamsBreakdown SetTeamsBreakdown(Match match)
        {
            var teamsBreakdown = new TeamsBreakdown();
            var blueTeam = match.Teams.FirstOrDefault(t => t.TeamId == 100);
            var redTeam = match.Teams.FirstOrDefault(t => t.TeamId == 200);

            var bluePlayers = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == 100);
            var redPlayers = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == 200);

            //Blue team stats
            teamsBreakdown.BlueTeamBaronKills = blueTeam.BaronKills;
            teamsBreakdown.BlueTeamDragonKills = blueTeam.DragonKills;
            teamsBreakdown.BlueTeamTowerKills = blueTeam.TowerKills;
            teamsBreakdown.BlueTeamChampionKills = bluePlayers.Select(p => p.Stats.Kills).ToList().Sum();
            teamsBreakdown.BlueTeamGold = bluePlayers.Select(p => p.Stats.GoldEarned).ToList().Sum();
            teamsBreakdown.BlueTeamDmgDealt = bluePlayers.Select(p => p.Stats.TotalDamageDealtToChampions).ToList().Sum();
            teamsBreakdown.BlueTeamWardsPlaced = bluePlayers.Select(p => p.Stats.WardsPlaced).ToList().Sum();
            teamsBreakdown.BlueTeamDmgTaken = bluePlayers.Select(p => p.Stats.TotalDamageTaken).ToList().Sum();
            teamsBreakdown.BlueTeamCS = bluePlayers.Select(p => p.Stats.TotalMinionsKilled).ToList().Sum();

            //Red team stats
            teamsBreakdown.RedTeamBaronKills = redTeam.BaronKills;
            teamsBreakdown.RedTeamDragonKills = redTeam.DragonKills;
            teamsBreakdown.RedTeamTowerKills = redTeam.TowerKills;
            teamsBreakdown.RedTeamChampionKills = redPlayers.Select(p => p.Stats.Kills).ToList().Sum();
            teamsBreakdown.RedTeamGold = redPlayers.Select(p => p.Stats.GoldEarned).ToList().Sum();
            teamsBreakdown.RedTeamDmgDealt = redPlayers.Select(p => p.Stats.TotalDamageDealtToChampions).ToList().Sum();
            teamsBreakdown.RedTeamWardsPlaced = redPlayers.Select(p => p.Stats.WardsPlaced).ToList().Sum();
            teamsBreakdown.RedTeamDmgTaken = redPlayers.Select(p => p.Stats.TotalDamageTaken).ToList().Sum();
            teamsBreakdown.RedTeamCS = redPlayers.Select(p => p.Stats.TotalMinionsKilled).ToList().Sum();

            return teamsBreakdown;
        }

        //For the match breakdown, return the rune paths so we have access to all the runes for display on the view, selected or unselected
        private static List<RunePath> GetRunesOfPlayer(Participant participant)
        {
            var pathBuilder = new UrlPathBuilder();

            var perkPrimaryStyle = RunePaths.FirstOrDefault(p => p.Id == participant.Stats.PerkPrimaryStyle);
            var perkSubStyle = RunePaths.FirstOrDefault(rp => rp.Id == participant.Stats.PerkSubStyle);
            var fragments = RunePaths.FirstOrDefault(rp => rp.Name == "Rune Stats");

            var runes = new List<RunePath>();
            runes.Add(perkPrimaryStyle);
            runes.Add(perkSubStyle);
            runes.Add(fragments);

            return runes;
        }

        private static List<Spell> GetChampionAbilities(string championId)
        {
            var pathBuilder = new UrlPathBuilder();
            var spells = new List<Spell>();

            using (var client = new WebClient())
            {
                try
                {
                    var championJson = client.DownloadString(pathBuilder.GetSpecificChampionUrl(championId));
                    var jsonObject = JObject.Parse(championJson);
                    var champSpells = jsonObject["data"][championId]["spells"];
                    spells = champSpells.ToObject<List<Spell>>();
                    foreach (var spell in spells)
                        spell.IconFullUrl = pathBuilder.GetSpellIcon(spell.Id);

                    return spells;
                }
                catch (Exception) { return new List<Spell>(); }
            }
        }

        private static EventsTimeline GetMatchTimeline(long gameId, string region, int participantId)
        {
            var pathBuilder = new UrlPathBuilder();
            var timeline = new EventsTimeline();
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetMatchTimelineUrl(gameId, region)));
                    response.Wait();

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var readData = response.Result.Content.ReadAsStringAsync();
                        readData.Wait();

                        var jsonObject = JObject.Parse(readData.Result);
                        foreach (var frame in jsonObject["frames"])
                        {
                            var events = frame.Children<JProperty>().FirstOrDefault(x => x.Name == "events");
                            var timestamp = frame.Children<JProperty>().FirstOrDefault(x => x.Name == "timestamp");

                            var intervalTimestamp = timestamp.Value.ToObject<long>();
                            var matchEvents = events.Value.ToObject<List<MatchEvent>>();

                            var eventsInsideTimestampWindow = new List<MatchEvent>();
                            foreach (var e in matchEvents)
                            {
                                if (e.Type == "ITEM_PURCHASED" && e.ParticipantId == participantId)
                                {
                                    e.ItemUrl = pathBuilder.GetItemIcon(e.ItemId);
                                    eventsInsideTimestampWindow.Add(e);
                                }
                                if (e.Type == "SKILL_LEVEL_UP" && e.ParticipantId == participantId)
                                {
                                    timeline.SkillEvents.Add(e);
                                }
                            }
                            timeline.ItemPurchaseEventsByTimestamp.Add(intervalTimestamp, eventsInsideTimestampWindow);
                        }
                    }
                    return timeline;
                }
                catch (Exception) { return new EventsTimeline(); }
            }
        }

        #endregion

        #region Other Utils
        private static ParticipantForDisplay SetParticipantForDisplay(Match match, Participant participant, ParticipantIdentity participantIdentity)
        {
            var pathBuilder = new UrlPathBuilder();
            var summonerSpells = GetSummonerSpellsPaths(participant);
            var runes = GetRunesPaths(participant);

            return new ParticipantForDisplay()
            {
                ChampionIconUrl = pathBuilder.GetChampionIconUrl(participant.ChampionId),
                SummonerSpell1IconUrl = pathBuilder.GetSummonerSpellIcon(summonerSpells.Item1.Replace("/lol-game-data/assets/", "").ToLower()),
                SummonerSpell2IconUrl = pathBuilder.GetSummonerSpellIcon(summonerSpells.Item2.Replace("/lol-game-data/assets/", "").ToLower()),
                RuneKeystoneIconUrl = runes.Item1,
                RuneSecondaryPathIconUrl = runes.Item2,
                KillParticipationPercentage = GetKillParticipation(match, participant),
                Items = GetItems(participant),
                ChampionName = Champions.FirstOrDefault(x => x.Key == participant.ChampionId.ToString()).Name,
                Participant = participant,
                ParticipantIdentity = participantIdentity
            };
        }

        private static int GetKillParticipation(Match match, Participant participant)
        {
            var team = match.ParticipantsByTeam.FirstOrDefault(t => t.Key == participant.TeamId);
            var totalTeamKills = 0;
            foreach (var player in team)
                totalTeamKills += player.Stats.Kills;

            return (int)Math.Round((double)(participant.Stats.Kills + participant.Stats.Assists) / totalTeamKills * 100);
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
            if (desc.Contains("Blind"))
                return "Blind";
            else if (desc.Contains("Draft"))
                return "Draft";
            else if (desc.Contains("Ranked Solo"))
                return "Ranked Solo";
            else if (desc.Contains("Ranked Flex"))
                return "Ranked Flex";
            else if (desc.Contains("ARAM"))
                return "Aram";

            return "Undetermined";
        }

        private static Tuple<string, string> GetRunesPaths(Participant participant)
        {
            var perkPrimaryStylePath = RunePaths.FirstOrDefault(rp => rp.Id == participant.Stats.PerkPrimaryStyle);
            var perkSubStylePath = RunePaths.FirstOrDefault(rp => rp.Id == participant.Stats.PerkSubStyle).Icon;
            var keystonePath = "";

            var found = false;
            foreach (var slot in perkPrimaryStylePath.Slots)
            {
                foreach (var rune in slot.Runes)
                {
                    if (rune.Id == participant.Stats.Perk0)
                    {
                        keystonePath = rune.Icon;
                        found = true;
                        break;
                    }
                }
                if (found == true)
                    break;
            }

            return Tuple.Create(keystonePath, perkSubStylePath);
        }

        private static Tuple<string, string> GetSummonerSpellsPaths(Participant participant)
        {
            var spell1Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell1Id).IconPath;
            var spell2Path = SummonerSpells.FirstOrDefault(s => s.Id == participant.Spell2Id).IconPath;

            return Tuple.Create(spell1Path, spell2Path);
        }

        //Gets the last time the summoner played
        public static string GetLastTimePlayedStr(MatchHistory matchHistory)
        {
            var matchTimestmap = matchHistory.Matches.First().Timestamp;

            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastMatchTime = date.AddMilliseconds(matchTimestmap).ToLocalTime();
            var difference = (DateTime.Now - lastMatchTime);

            if (difference.Days > 0)
            {
                return "Played " + (difference.Days == 1 ? difference.Days + " day" : difference.Days + " days") + " ago";
            }
            else if (difference.Days == 0)
            {
                if (difference.Hours == 0)
                    return "Played " + (difference.Minutes == 1 ? difference.Minutes + " minute" : difference.Minutes + " minutes") + " ago";
                else if (difference.Hours == 1)
                    return "Played " + difference.Hours + " hour ago";
                else
                    return "Played " + difference.Hours + " hours ago";
            }

            return "Played " + difference.Hours + " hours ago";
        }

        //Gets how long ago each match was played
        private static string GetMatchWasPlayedTime(long timestamp)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastMatchTime = date.AddMilliseconds(timestamp).ToLocalTime();
            var difference = (DateTime.Now - lastMatchTime);

            if (difference.Days > 0)
            {
                return (difference.Days == 1 ? difference.Days + " day" : difference.Days + " days") + " ago";
            }
            else if (difference.Days == 0)
            {
                if (difference.Hours == 0)
                    return (difference.Minutes == 1 ? difference.Minutes + " minute" : difference.Minutes + " minutes") + " ago";
                else if (difference.Hours == 1)
                    return difference.Hours + " hour ago";
                else
                    return difference.Hours + " hours ago";
            }

            return difference.Hours + " hours ago";
        }


        private static string GetGameDurationInText(long gameDuration)
        {
            TimeSpan time = TimeSpan.FromSeconds(gameDuration);
            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }

        public static void SetLeagueEntriesWinRates(List<LeagueEntry> leagueEntries)
        {
            foreach (var entry in leagueEntries)
                entry.WinRate = (int)Math.Round((double)entry.Wins / (entry.Wins + entry.Losses) * 100);
        }

        #endregion

        #region Fetch static data methods
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
                    var runePaths = JsonConvert.DeserializeObject<List<RunePath>>(runesJson);

                    foreach (var path in runePaths)
                    {
                        path.Icon = pathBuilder.GetRuneIcon(path.Icon);
                        foreach (var slot in path.Slots)
                            foreach (var rune in slot.Runes)
                                rune.Icon = pathBuilder.GetRuneIcon(rune.Icon);
                    }

                    //Deliberately adding a custom runepath for rune fragments, since they are not provided by DDragon
                    var runepath = new RunePath() { Name = "Rune Stats" };
                    var slot1 = new Slot();
                    slot1.Runes.Add(new Rune { Id = 5008, Name = "Offense", LongDesc = "Adaptive Force +9", ShortDesc = "Adaptive Force +9", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_dmg.png" }); // damage fragment
                    slot1.Runes.Add(new Rune { Id = 5005, Name = "Offense", LongDesc = "+10% Attack Speed", ShortDesc = "+10% Attack Speed", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_aspeed.png" }); // attack speed fragment
                    slot1.Runes.Add(new Rune { Id = 5007, Name = "Offense", LongDesc = "+1-10% CDR (based on level)", ShortDesc = "+1-10% CDR (based on level)", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_cdr.png" }); // cdr fragment
                    var slot2 = new Slot();
                    slot2.Runes.Add(new Rune { Id = 5008, Name = "Flex", LongDesc = "Adaptive Force +9", ShortDesc = "Adaptive Force +9", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_dmg.png" }); // damage fragment
                    slot2.Runes.Add(new Rune { Id = 5002, Name = "Flex", LongDesc = "+6 Armor", ShortDesc = "+6 Armor", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_armor.png" }); // armor fragment
                    slot2.Runes.Add(new Rune { Id = 5003, Name = "Flex", LongDesc = "+8 Magic Resist", ShortDesc = "+8 Magic Resist", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_mr.png" }); // magic resist fragment
                    var slot3 = new Slot();
                    slot3.Runes.Add(new Rune { Id = 5001, Name = "Defense", LongDesc = "+15-90 Health (Based on level)", ShortDesc = "+15-90 Health (Based on level)", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_hp.png" }); // hp fragment
                    slot3.Runes.Add(new Rune { Id = 5002, Name = "Defense", LongDesc = "+6 Armor", ShortDesc = "+6 Armor", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_armor.png" }); // armor fragment
                    slot3.Runes.Add(new Rune { Id = 5003, Name = "Defense", LongDesc = "+8 Magic Resist", ShortDesc = "+8 Magic Resist", Icon = "../DataAccess/RiotGamesApi/Images/Misc/fragment_mr.png" }); // magic resist fragment

                    runepath.Slots.Add(slot1);
                    runepath.Slots.Add(slot2);
                    runepath.Slots.Add(slot3);

                    runePaths.Add(runepath);

                    return runePaths;
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
                            Key = championProperties["key"].Value<string>(),
                            Id = championProperties["id"].Value<string>()
                        });
                    }

                    return championList;
                }
                catch (Exception) { return new List<Champion>(); }
            }
        }

        #endregion
    }
}