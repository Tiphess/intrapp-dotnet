using intrapp.DataAccess;
using intrapp.DataAccess.RiotGamesApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace intrapp.Models.DLL
{
    public class DLL_Leaderboards
    {
        public League GetChallengers(string queue, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var league = new League();

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetChallengerLeaguesUrl(queue, region)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        league = JsonConvert.DeserializeObject<League>(readData.Result);
                        league.Entries = league.Entries.OrderByDescending(e => e.LeaguePoints).ToList();
                        league.Pages = (league.Entries.Count >= League.PageSize ? league.Entries.Count : League.PageSize) / League.PageSize;
                    }
                }
                catch (Exception) { return new League(); }
            }
            return league;
        }

        public League GetGrandmasters(string queue, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var league = new League();

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetGrandmasterLeaguesUrl(queue, region)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        league = JsonConvert.DeserializeObject<League>(readData.Result);
                        league.Entries = league.Entries.OrderByDescending(e => e.LeaguePoints).ToList();
                        league.Pages = (league.Entries.Count >= League.PageSize ? league.Entries.Count : League.PageSize) / League.PageSize;
                    }
                }
                catch (Exception) { return new League(); }
            }
            return league;

        }

        public League GetMasters(string queue, string region)
        {
            var pathBuilder = new UrlPathBuilder();
            var league = new League();

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Riot-Token", ConfigWrapper.ApiKey);
                    var response = client.GetAsync(new Uri(pathBuilder.GetMasterLeaguesUrl(queue, region)));
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readData = result.Content.ReadAsStringAsync();
                        readData.Wait();

                        league = JsonConvert.DeserializeObject<League>(readData.Result);
                        league.Entries = league.Entries.OrderByDescending(e => e.LeaguePoints).ToList();
                        league.Pages = (league.Entries.Count >= League.PageSize ? league.Entries.Count : League.PageSize) / League.PageSize;
                    }
                }
                catch (Exception) { return new League(); }
            }
            return league;
        }
    }
}