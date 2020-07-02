using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using WebGrease.Css.Ast.Selectors;
using Newtonsoft.Json;
//using intrapp.DataAccess;

namespace intrapp.DataAccess.RiotGamesApi
{
    internal static class PlatformProvider
    {
        internal readonly static Dictionary<string, string> PLATFORMS = new Dictionary<string, string>
        {
            { "BR1", "https://br1.api.riotgames.com" },
            { "EUN1", "https://eun1.api.riotgames.com" },
            { "EUW1", "https://euw1.api.riotgames.com" },
            { "JP1", "https://jp1.api.riotgames.com" },
            { "KR", "https://kr.api.riotgames.com" },
            { "LA1", "https://la1.api.riotgames.com" },
            { "LA2", "https://la2.api.riotgames.com" },
            { "NA1", "https://na1.api.riotgames.com" },
            { "OC1", "https://oc1.api.riotgames.com" },
            { "TR1", "https://tr1.api.riotgames.com" },
            { "RU", "https://ru.api.riotgames.com" },
        };

        internal readonly static Dictionary<string, string> REGIONS = new Dictionary<string, string>
        {
            { "AMERICAS", "https://americas.api.riotgames.com" },
            { "ASIA", "https://asia.api.riotgames.com" },
            { "EUROPE", "https://europe.api.riotgames.com" },
        };
    }

    internal class StaticDataPathProvider
    {
        //DataDragon - Riot's official static data API
        //docs https://developer.riotgames.com/docs/lol#data-dragon
        internal readonly string GET_DDRAGON_VERSIONS = "https://ddragon.leagueoflegends.com/api/versions.json";
        internal readonly string DDRAGON_GET = "https://ddragon.leagueoflegends.com/cdn/{version}";
        internal readonly string DDRAGON_PROFILEICON = "/img/profileicon/{profileIconId}.png";
        internal readonly string DDRAGON_CHAMPIONICON = "/img/champion/";
        internal readonly string DDRAGON_SUMMONERSPELL = "/img/spell/";
        internal readonly string DDRAGON_VERSIONLESS_IMG = "https://ddragon.leagueoflegends.com/cdn/img/{path}"; // Versionless is used for some icons
        internal readonly string DDRAGON_RUNES_DATA = "/data/en_US/runesReforged.json";
        internal readonly string DDRAGON_ITEMS_DATA = "/data/en_US/item.json";
        internal readonly string DDRAGON_CHAMPION_DATA = "/data/en_US/champion.json";
        internal readonly string DDRAGON_SPECIFIC_CHAMPION_DATA = "/data/en_US/champion/{championId}.json";
        internal readonly string DDRAGON_ITEM_ICON = "/img/item/{itemId}.png";
        internal readonly string DDRAGON_CHAMPION_SPELL = "/img/spell/{spellId}.png";

        //CommunityDragon URLs - Community made custom API to retrieve game data more easily and much more quickly
        //docs https://www.communitydragon.org/docs
        internal readonly string CDRAGON_GET_CHAMPION_ICON = "https://cdn.communitydragon.org/latest/champion/{championId}/square";
        internal readonly string CDRAGON_PREFIX = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/{path}";
        internal readonly string CDRAGON_SUMMONERSPELLS_JSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/summoner-spells.json";
        internal readonly string CDRAGON_RUNESREFORGED_JSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/perks.json";
    }

    internal static class ApiPathProvider
    {
        internal readonly static string GET_SUMMONER_BY_NAME = "/lol/summoner/v4/summoners/by-name/{summonerName}";
        internal readonly static string GET_MATCH_HISTORY_BY_ACCOUNTID = "/lol/match/v4/matchlists/by-account/{accountId}";
        internal readonly static string GET_MATCH_BY_GAMEID = "/lol/match/v4/matches/{gameId}";
        internal readonly static string GET_LEAGUE_ENTRY_BY_SUMMONERID = "/lol/league/v4/entries/by-summoner/{summonerId}";
        internal readonly static string GET_MATCH_TIMELINE_BY_GAMEID = "/lol/match/v4/timelines/by-match/{matchId}";
        internal readonly static string GET_CHALLENGER_LEAGUES = "/lol/league/v4/challengerleagues/by-queue/{queue}";
        internal readonly static string GET_GRANDMASTER_LEAGUES = "/lol/league/v4/grandmasterleagues/by-queue/{queue}";
        internal readonly static string GET_MASTER_LEAGUES = "/lol/league/v4/masterleagues/by-queue/{queue}";

        internal readonly static string DEFAULT_ENDINDEX = "?endIndex=5";
    }
}