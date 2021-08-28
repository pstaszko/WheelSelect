﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;

namespace WheelSelect
{
    public class Settings
    {
        public string OutputLocation { get; set; }
        public bool ClearOutputFileOnStart { get; set; }
        public bool ClearOutputFileOnEscape { get; set; }
        public OutputMethodEnum OutputMethod { get; set; }
        public bool SyncWithWindowsTheme { get; set; }
        public WindowsTheme DefaultTheme { get; set; }
        public Color LightModeBackgroundColor { get; set; }
        public Color LightModeSelectedTextColor { get; set; }
        public Color LightModeOffset1TextColor { get; set; }
        public Color LightModeOffset2TextColor { get; set; }
        public Color LightModeOffset3TextColor { get; set; }
        public Color DarkModeBackgroundColor { get; set; }
        public Color DarkModeSelectedTextColor { get; set; }
        public Color DarkModeOffset1TextColor { get; set; }
        public Color DarkModeOffset2TextColor { get; set; }
        public Color DarkModeOffset3TextColor { get; set; }

        private bool configToBool(string cfg, bool def)
        {
            if (String.IsNullOrEmpty(cfg)) {
                return def;
            } else {
                bool parsed;
                if (bool.TryParse(cfg, out parsed)) {
                    return parsed;
                } else {
                    return def;
                }
            }
        }
        private OutputMethodEnum configToOutputMethodEnum(string cfg, OutputMethodEnum def)
        {
            if (String.IsNullOrEmpty(cfg)) {
                return def;
            } else {
                OutputMethodEnum parsed;
                if (Enum.TryParse(cfg, out parsed)) {
                    return parsed;
                } else {
                    return def;
                }
            }
        }
        private WindowsTheme configToWindowsTheme(string cfg, WindowsTheme def)
        {
            if (String.IsNullOrEmpty(cfg)) {
                return def;
            } else {
                WindowsTheme parsed;
                if (Enum.TryParse(cfg, out parsed)) {
                    return parsed;
                } else {
                    return def;
                }
            }
        }
        private Color configToColor(string cfg, Color def)
        {
            if (String.IsNullOrEmpty(cfg)) {
                return def;
            } else {
                try {
                    return Color.FromName(cfg);
                } catch (Exception) {
                    return def;
                }
            }
        }

        public Settings GetSettings()
        {
            string outputLocation = ConfigurationManager.AppSettings["OutputLocation"] ?? @"c:\wheel_selection.txt";
            bool clearOutputFileOnStart = configToBool(ConfigurationManager.AppSettings["ClearOutputFileOnStart"], true);
            bool clearOutputFileOnEscape = configToBool(ConfigurationManager.AppSettings["ClearOutputFileOnEscape"], true);
            bool syncWithWindowsTheme = configToBool(ConfigurationManager.AppSettings["SyncWithWindowsTheme"], true);
            OutputMethodEnum outputMethod = configToOutputMethodEnum(ConfigurationManager.AppSettings["OutputMethod"], OutputMethodEnum.Overwrite);
            WindowsTheme defaultTheme = configToWindowsTheme(ConfigurationManager.AppSettings["DefaultTheme"], WindowsTheme.Default);
            Color lightModeWindowBackgroundColor = configToColor(ConfigurationManager.AppSettings["LightModeWindowBackgroundColor"], Color.White);
            Color lightModeSelectedTextColor = configToColor(ConfigurationManager.AppSettings["LightModeSelectedTextColor"], Color.SteelBlue);
            Color lightModeOffset1TextColor = configToColor(ConfigurationManager.AppSettings["LightModeOffset1TextColor"], Color.Silver);
            Color lightModeOffset2TextColor = configToColor(ConfigurationManager.AppSettings["LightModeOffset2TextColor"], Color.Gray);
            Color lightModeOffset3TextColor = configToColor(ConfigurationManager.AppSettings["LightModeOffset3TextColor"], Color.DimGray);
            Color darkModeWindowBackgroundColor = configToColor(ConfigurationManager.AppSettings["DarkModeWindowBackgroundColor"], Color.Black);
            Color darkModeSelectedTextColor = configToColor(ConfigurationManager.AppSettings["DarkModeSelectedTextColor"], Color.White);
            Color darkModeOffset1TextColor = configToColor(ConfigurationManager.AppSettings["DarkModeOffset1TextColor"], Color.Silver);
            Color darkModeOffset2TextColor = configToColor(ConfigurationManager.AppSettings["DarkModeOffset2TextColor"], Color.Gray);
            Color darkModeOffset3TextColor = configToColor(ConfigurationManager.AppSettings["DarkModeOffset3TextColor"], Color.DimGray);
            return new Settings() {
                OutputLocation = outputLocation,
                ClearOutputFileOnStart = clearOutputFileOnStart,
                ClearOutputFileOnEscape = clearOutputFileOnEscape,
                OutputMethod = outputMethod,
                SyncWithWindowsTheme = syncWithWindowsTheme,
                DefaultTheme = defaultTheme,
                LightModeBackgroundColor = lightModeWindowBackgroundColor,
                LightModeSelectedTextColor = lightModeSelectedTextColor,
                LightModeOffset1TextColor = lightModeOffset1TextColor,
                LightModeOffset2TextColor = lightModeOffset2TextColor,
                LightModeOffset3TextColor = lightModeOffset3TextColor,
                DarkModeBackgroundColor = darkModeWindowBackgroundColor,
                DarkModeSelectedTextColor = darkModeSelectedTextColor,
                DarkModeOffset1TextColor = darkModeOffset1TextColor,
                DarkModeOffset2TextColor = darkModeOffset2TextColor,
                DarkModeOffset3TextColor = darkModeOffset3TextColor,
            };
        }
    }

    public enum OutputMethodEnum
    {
        Overwrite,
        Append
    }
}