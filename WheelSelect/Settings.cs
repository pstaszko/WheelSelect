using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WheelSelect
{
    public class Settings
    {
        public string Delimiter { get; set; }
        public List<string> Data { get; set; }
        public string OutputLocation { get; set; }
        public bool ClearOutputFileOnStart { get; set; }
        public bool ClearOutputFileOnEscape { get; set; }
        public OutputMethodEnum OutputMethod { get; set; }
        public bool SyncWithWindowsTheme { get; set; }
        public WindowsTheme DefaultTheme { get; set; }
        public Color LightModeBackgroundColor { get; set; }
        public Color LightModeBorderColor { get; set; }
        public Color LightModeInputTextColor { get; set; }
        public Color LightModeSelectedTextColor { get; set; }
        public Color LightModeOffset1TextColor { get; set; }
        public Color LightModeOffset2TextColor { get; set; }
        public Color LightModeOffset3TextColor { get; set; }
        public Color DarkModeBackgroundColor { get; set; }
        public Color DarkModeBorderColor { get; set; }
        public Color DarkModeInputTextColor { get; set; }
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
        private Color configHexColor(string cfg, Color def)
        {
            if (String.IsNullOrEmpty(cfg)) {
                return def;
            } else {
                try {
                    return ColorTranslator.FromHtml(cfg);
                } catch (Exception) {
                    return def;
                }
            }
        }
        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg
                    , "Something went wrong"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
            Application.Exit();
            Environment.Exit(1);
        }

        private string[] getArgs()
        {
            // args[0] seems to always be the execution path. Neat.
            return Environment.GetCommandLineArgs();
        }

        private bool isNull(string str)
        {
            return (String.IsNullOrEmpty(str) || String.IsNullOrWhiteSpace(str));
        }

        private string getOutputFilePath()
        {
            var args = getArgs();
            var defaultOutput = ConfigurationManager.AppSettings["OutputLocation"] ?? @"c:\wheel_selection.txt";
            var output = "";
            try {
                output = args[3];
            } catch (Exception) {
                output = defaultOutput;
            }
            if (isNull(output)) {
                ShowErrorMessage("Output location cannot be an empty string.\n\nEither do not specify a location in the arguments or provide a valid path.");
            }
            return output;
        }

        private string getDelimeter()
        {
            var args = getArgs();
            var delim = "";
            try {
                delim = args[1];
            } catch (Exception) {
                ShowErrorMessage("Missing the delimeter parameter.");
            }
            if (delim == "\\n") {
                delim = "\n";
            }
            return delim;
        }

        private List<string> getData(string delim)
        {
            var args = getArgs();
            var data = "";
            try {
                data = args[2];
            } catch (Exception) {
                ShowErrorMessage("Missing the data parameter.");
            }
            data = data.Replace("\\n", "\n");
            var dataArray = Regex.Split(data, delim);
            return dataArray.ToList();
        }

        public Settings GetSettings()
        {
            string outputLocation = getOutputFilePath();
            string delim = getDelimeter();
            List<string> data = getData(delim);
            bool clearOutputFileOnStart = configToBool(ConfigurationManager.AppSettings["ClearOutputFileOnStart"], true);
            bool clearOutputFileOnEscape = configToBool(ConfigurationManager.AppSettings["ClearOutputFileOnEscape"], true);
            bool syncWithWindowsTheme = configToBool(ConfigurationManager.AppSettings["SyncWithWindowsTheme"], true);
            OutputMethodEnum outputMethod = configToOutputMethodEnum(ConfigurationManager.AppSettings["OutputMethod"], OutputMethodEnum.Overwrite);
            WindowsTheme defaultTheme = configToWindowsTheme(ConfigurationManager.AppSettings["DefaultTheme"], WindowsTheme.Default);
            Color lightModeWindowBackgroundColor = configToColor(ConfigurationManager.AppSettings["LightModeWindowBackgroundColor"], Color.White);
            Color lightModeBorderColor = configHexColor(ConfigurationManager.AppSettings["LightModeBorderColor"], Color.White);
            Color lightModeInputTextColor = configHexColor(ConfigurationManager.AppSettings["LightModeInputTextColor"], Color.SteelBlue);
            Color lightModeSelectedTextColor = configHexColor(ConfigurationManager.AppSettings["LightModeSelectedTextColor"], Color.SteelBlue);
            Color lightModeOffset1TextColor = configHexColor(ConfigurationManager.AppSettings["LightModeOffset1TextColor"], Color.Silver);
            Color lightModeOffset2TextColor = configHexColor(ConfigurationManager.AppSettings["LightModeOffset2TextColor"], Color.Gray);
            Color lightModeOffset3TextColor = configHexColor(ConfigurationManager.AppSettings["LightModeOffset3TextColor"], Color.DimGray);
            Color darkModeWindowBackgroundColor = configHexColor(ConfigurationManager.AppSettings["DarkModeWindowBackgroundColor"], Color.Black);
            Color darkModeBorderColor = configHexColor(ConfigurationManager.AppSettings["DarkModeBorderColor"], Color.Black);
            Color darkModeInputTextColor = configHexColor(ConfigurationManager.AppSettings["DarkModeInputTextColor"], Color.White);
            Color darkModeSelectedTextColor = configHexColor(ConfigurationManager.AppSettings["DarkModeSelectedTextColor"], Color.White);
            Color darkModeOffset1TextColor = configHexColor(ConfigurationManager.AppSettings["DarkModeOffset1TextColor"], Color.Silver);
            Color darkModeOffset2TextColor = configHexColor(ConfigurationManager.AppSettings["DarkModeOffset2TextColor"], Color.Gray);
            Color darkModeOffset3TextColor = configHexColor(ConfigurationManager.AppSettings["DarkModeOffset3TextColor"], Color.DimGray);
            return new Settings() {
                OutputLocation = outputLocation,
                Delimiter = delim,
                Data = data,
                ClearOutputFileOnStart = clearOutputFileOnStart,
                ClearOutputFileOnEscape = clearOutputFileOnEscape,
                OutputMethod = outputMethod,
                SyncWithWindowsTheme = syncWithWindowsTheme,
                DefaultTheme = defaultTheme,
                LightModeBackgroundColor = lightModeWindowBackgroundColor,
                LightModeBorderColor = lightModeBorderColor,
                LightModeInputTextColor = lightModeInputTextColor,
                LightModeSelectedTextColor = lightModeSelectedTextColor,
                LightModeOffset1TextColor = lightModeOffset1TextColor,
                LightModeOffset2TextColor = lightModeOffset2TextColor,
                LightModeOffset3TextColor = lightModeOffset3TextColor,
                DarkModeBackgroundColor = darkModeWindowBackgroundColor,
                DarkModeBorderColor = darkModeBorderColor,
                DarkModeInputTextColor = darkModeInputTextColor,
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
    public enum Position
    {
        Top3,
        Top2,
        Top1,
        Primary,
        Bottom1,
        Bottom2,
        Bottom3
    }
    public enum WindowsTheme
    {
        Default,
        LightMode,
        DarkMode
    }
}