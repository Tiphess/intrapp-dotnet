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

    internal static class DataDragonUrlPaths
    {
        internal readonly static string GET_DDRAGON_VERSIONS = "https://ddragon.leagueoflegends.com/api/versions.json";
        internal readonly static string DDRAGON_BASE_CDN = "https://ddragon.leagueoflegends.com/cdn/";
        internal readonly static string DDRAGON_PROFILEICON = "img/profileicon/";
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