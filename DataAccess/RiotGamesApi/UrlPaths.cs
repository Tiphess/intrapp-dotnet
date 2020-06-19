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
    internal static class BaseUrlPaths
    {
        internal readonly static string HTTPS = "https://";
        internal readonly static Dictionary<string, string> PLATFORMS = new Dictionary<string, string>
        {
            { "BR1", "br1.api.riotgames.com" },
            { "EUN1", "eun1.api.riotgames.com" },
            { "EUW1", "euw1.api.riotgames.com" },
            { "JP1", "jp1.api.riotgames.com" },
            { "KR", "kr.api.riotgames.com" },
            { "LA1", "la1.api.riotgames.com" },
            { "LA2", "la2.api.riotgames.com" },
            { "NA1", "na1.api.riotgames.com" },
            { "OC1", "oc1.api.riotgames.com" },
            { "TR1", "tr1.api.riotgames.com" },
            { "RU", "ru.api.riotgames.com" },
        };

        internal readonly static Dictionary<string, string> REGIONS = new Dictionary<string, string>
        {
            { "AMERICAS", "americas.api.riotgames.com" },
            { "ASIA", "asia.api.riotgames.com" },
            { "EUROPE", "europe.api.riotgames.com" },
        };

        internal readonly static string DEFAULT_PLATFORM = "NA1";
    }

    internal class DataDragonUrlPaths
    {
        internal readonly string GET_DDRAGON_VERSIONS = "https://ddragon.leagueoflegends.com/api/versions.json";
        internal readonly string DDRAGON_BASE_CDN = "https://ddragon.leagueoflegends.com/cdn/";
        internal readonly string DDRAGON_PROFILEICON = "img/profileicon/";
        internal readonly string DDRAGON_CHAMPIONICON = "img/champion/";
        internal readonly string DDRAGON_SUMMONERSPELL = "img/spell/";
        internal readonly string DDRAGON_CHAMPION_DATA = "data/en_US/champion.json";
        internal readonly string DDRAGON_VERSIONLESS_IMG = "https://ddragon.leagueoflegends.com/cdn/img/"; // For runes icons

        //Runes Reforged
        internal readonly string DDRAGON_RUNES_JSON = "data/en_US/runesReforged.json"; 

        //CommunityDragon URLs - Community made custom API to retrieve game data more easily and much more quickly
        //Full url - Get a champion's icon => https://cdn.communitydragon.org/{version}/champion/{championId}/square
        internal readonly string CDRAGON_BASE = "https://cdn.communitydragon.org/";
        internal readonly string CDRAGON_CHAMPION = "/champion/";
        internal readonly string CDRAGON_ICON = "/square";
        internal readonly string CDRAGON_PREFIX = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/";

        //Summoner spells
        internal readonly string CDRAGON_SUMMONERSPELLS_JSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/summoner-spells.json";
    }

    internal static class ApiUrlPaths
    {
        internal readonly static string GET_SUMMONER_BY_NAME = "/lol/summoner/v4/summoners/by-name/";
        internal readonly static string GET_MATCH_HISTORY_BY_ACCOUNTID = "/lol/match/v4/matchlists/by-account/";
        internal readonly static string GET_MATCH_BY_GAMEID = "/lol/match/v4/matches/";
        internal readonly static string GET_LEAGUE_ENTRY_BY_SUMMONERID = "/lol/league/v4/entries/by-summoner/";

        internal readonly static string PARAMETER_DEFAULT_ENDINDEX = "?endIndex=5";
    }
}