﻿using System;
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
                        return "lightgray";
                }
            }
        }

        public static bool ShowEnvironmentColor => !string.IsNullOrWhiteSpace(EnvironmentColor);

        public static string BrmoApplicationBaseUrl => ConfigurationManager.AppSettings["BrmoApplicationBaseUrl"];

        public static bool BrmoApplicationEnabled => !string.IsNullOrWhiteSpace(BrmoApplicationBaseUrl);

        public static string UsernameToUseWhenEmpty => ConfigurationManager.AppSettings["UsernameToUseWhenEmpty"];

        public static bool UseHardCodedUserManagerForTesting =>
            Convert.ToBoolean(ConfigurationManager.AppSettings["UseHardCodedUserManagerForTesting"] ?? "False");

        public static bool EnsureAuthenticatedUser => Convert.ToBoolean(ConfigurationManager.AppSettings["EnsureAuthenticatedUser"]);

        public static List<string> InitialUserAdministrators =>
            ConfigurationManager.AppSettings["InitialUserAdministrators"]
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        public static List<string[]> ADDistinguishedNameFilters =>
            (ConfigurationManager.AppSettings["ADDistinguishedNameFilters"] ?? string.Empty)
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f=> f.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList();

        public static string UserManagerPrefillUserName => ConfigurationManager.AppSettings["UserManagerPrefillUserName"] ?? string.Empty;
    }
}