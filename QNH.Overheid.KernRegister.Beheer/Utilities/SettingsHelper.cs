using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace QNH.Overheid.KernRegister.Beheer.Utilities
{
    public static class SettingsHelper
    {
        public static string Environment => ConfigurationManager.AppSettings["Environment"] ?? "PROD";

        public static string EnvironmentColor
        {
            get
            {
                switch (Environment)
                {
                    case "DEV":
                        return "lightskyblue";
                    case "ACC":
                        return "#388f58";
                    default:
                        return string.Empty;
                }
            }
        }

        public static bool ShowEnvironmentColor => !string.IsNullOrWhiteSpace(EnvironmentColor);

        public static string BrmoApplicationBaseUrl => ConfigurationManager.AppSettings["BrmoApplicationBaseUrl"];

        public static bool BrmoApplicationEnabled => !string.IsNullOrWhiteSpace(BrmoApplicationBaseUrl);
    }
}