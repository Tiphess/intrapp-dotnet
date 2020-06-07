using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace intrapp.DataAccess
{
    public static class ConfigWrapper
    {
        public static readonly string ApiKey = WebConfigurationManager.AppSettings["RIOT_GAMES_API_DEV_KEY"];
        public static readonly string AppName = WebConfigurationManager.AppSettings["APPLICATION_NAME"];
    }
}