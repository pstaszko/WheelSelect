using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WheelSelect
{
    public partial class WheelSelect : Form
    {
        private List<string> theList = new List<string>();
        private int selectedIndex = 0;
        private Settings settings;
        private string prevInputQueue = "";
        private string inputQueue = "";
        private Timer processInputQueueTimer = new Timer();
        private Timer clearInputQueueTimer = new Timer();

        public WheelSelect()
        {
            InitializeComponent();
            processInputQueueTimer.Interval = 500;
            clearInputQueueTimer.Interval = 800;
            processInputQueueTimer.Tick += ProcessInputQueueTimer_Tick;
            clearInputQueueTimer.Tick += ClearInputQueueTimer_Tick;
            this.MouseWheel += Form1_MouseWheel;
            settings = new Settings().GetSettings();
        }

        private void ClearInputQueueTimer_Tick(object sender, EventArgs e)
        {
            inputQueue = "";
            prevInputQueue = "";
            clearInputQueueTimer.Stop();
        }

        private void ProcessInputQueueTimer_Tick(object sender, EventArgs e)
        {
            if (!inputQueue.Equals("") && !inputQueue.Equals(prevInputQueue)) {
                TryFindMatch(inputQueue);
                prevInputQueue = inputQueue;
                clearInputQueueTimer.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (settings.ClearOutputFileOnStart) {
                WriteToSaveFile("");
            }
            processInputQueueTimer.Start();
            TrySetColors();
            HandleArgs();
            HandleEvent(false);
        }

        private void HandleArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            // args[0] seems to always = execution path. Neat.
            var delim = args[1];
            var data = args[2];
            if (delim == "\\n") {
                delim = "\n";
                data = data.Replace("\\n", "\n");
            }
            var splitData = Regex.Split(data, delim);
            theList.AddRange(splitData);
            selectedIndex = -1;
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Debug.WriteLine(e.KeyCode);
            if (e.KeyCode == Keys.Escape) {
                if (settings.ClearOutputFileOnEscape) {
                    WriteToSaveFile("");
                }
                Application.Exit();
            } else if (e.KeyCode == Keys.Enter) {
                SaveSelection();
            } else if (e.KeyCode == Keys.Up) {
                HandleEvent(true);
            } else if (e.KeyCode == Keys.Down) {
                HandleEvent(false);
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) {
                HandleEvent(true);
            } else {
                HandleEvent(false);
            }
        }

        private void HandleEvent(bool up)
        {
            if (up) {
                selectedIndex = GetPreviousIndex();
            } else {
                selectedIndex = GetNextIndex();
            }
            top3.Text = SetText(Position.Top3);
            top2.Text = SetText(Position.Top2);
            top1.Text = SetText(Position.Top1);
            selected.Text = SetText(Position.Primary);
            bottom1.Text = SetText(Position.Bottom1);
            bottom2.Text = SetText(Position.Bottom2);
            bottom3.Text = SetText(Position.Bottom3);
        }

        private string SetText(Position pos)
        {
            var trackInt = 0;
            switch (pos) {
                case Position.Top3:
                    trackInt = selectedIndex - 3;
                    break;
                case Position.Top2:
                    trackInt = selectedIndex - 2;
                    break;
                case Position.Top1:
                    trackInt = selectedIndex - 1;
                    break;
                case Position.Primary:
                    trackInt = selectedIndex;
                    break;
                case Position.Bottom1:
                    trackInt = selectedIndex + 1;
                    break;
                case Position.Bottom2:
                    trackInt = selectedIndex + 2;
                    break;
                case Position.Bottom3:
                    trackInt = selectedIndex + 3;
                    break;
                default:
                    trackInt = selectedIndex;
                    break;
            }
            if (trackInt > theList.Count() - 1) {
                return "";
            }
            if (trackInt < 0) {
                return "";
            }
            return theList[trackInt];
        }

        private int GetPreviousIndex()
        {
            if (selectedIndex == 0) {
                return 0;
            } else if (selectedIndex == theList.Count()) {
                return selectedIndex;
            }
            return selectedIndex - 1;
        }

        private int GetNextIndex()
        {
            if (selectedIndex == theList.Count() - 1) {
                return selectedIndex;
            }
            return selectedIndex + 1;
        }

        private void SaveSelection()
        {
            WriteToSaveFile(selected.Text);
            Environment.Exit(0);
        }

        private void WriteToSaveFile(string value)
        {
            switch (settings.OutputMethod) {
                case OutputMethodEnum.Overwrite:
                    File.WriteAllText(settings.OutputLocation, value);
                    break;
                case OutputMethodEnum.Append:
                    File.AppendAllText(settings.OutputLocation, value);
                    break;
                default:
                    File.WriteAllText(settings.OutputLocation, value);
                    break;
            }
        }

        private WindowsTheme GetWindowsThemeSetting()
        {
            int def = 0;
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize").GetValue("AppsUseLightTheme", "0").ToString();
            if (Int32.TryParse(key, out def)) {
                if (def == 1) {
                    return WindowsTheme.LightMode;
                } else {
                    return WindowsTheme.DarkMode;
                }
            }
            return WindowsTheme.Default;
        }

        private void TrySetColors()
        {
            if (settings.SyncWithWindowsTheme) {
                var theme = GetWindowsThemeSetting();
                switch (theme) {
                    case WindowsTheme.LightMode:
                        this.BackColor = settings.LightModeBackgroundColor;
                        this.selected.ForeColor = settings.LightModeSelectedTextColor;
                        this.top1.ForeColor = settings.LightModeOffset1TextColor;
                        this.bottom1.ForeColor = settings.LightModeOffset1TextColor;
                        this.top2.ForeColor = settings.LightModeOffset2TextColor;
                        this.bottom2.ForeColor = settings.LightModeOffset2TextColor;
                        this.top3.ForeColor = settings.LightModeOffset3TextColor;
                        this.bottom3.ForeColor = settings.LightModeOffset3TextColor;
                        break;
                    case WindowsTheme.DarkMode:
                        this.BackColor = settings.DarkModeBackgroundColor;
                        this.selected.ForeColor = settings.DarkModeSelectedTextColor;
                        this.top1.ForeColor = settings.DarkModeOffset1TextColor;
                        this.bottom1.ForeColor = settings.DarkModeOffset1TextColor;
                        this.top2.ForeColor = settings.DarkModeOffset2TextColor;
                        this.bottom2.ForeColor = settings.DarkModeOffset2TextColor;
                        this.top3.ForeColor = settings.DarkModeOffset3TextColor;
                        this.bottom3.ForeColor = settings.DarkModeOffset3TextColor;
                        break;
                    default:
                        this.BackColor = settings.LightModeBackgroundColor;
                        this.selected.ForeColor = settings.LightModeSelectedTextColor;
                        this.top1.ForeColor = settings.LightModeOffset1TextColor;
                        this.bottom1.ForeColor = settings.LightModeOffset1TextColor;
                        this.top2.ForeColor = settings.LightModeOffset2TextColor;
                        this.bottom2.ForeColor = settings.LightModeOffset2TextColor;
                        this.top3.ForeColor = settings.LightModeOffset3TextColor;
                        this.bottom3.ForeColor = settings.LightModeOffset3TextColor;
                        break;
                }
            } else {
                if (settings.DefaultTheme == WindowsTheme.DarkMode) {
                    this.BackColor = settings.DarkModeBackgroundColor;
                    this.selected.ForeColor = settings.DarkModeSelectedTextColor;
                    this.top1.ForeColor = settings.DarkModeOffset1TextColor;
                    this.bottom1.ForeColor = settings.DarkModeOffset1TextColor;
                    this.top2.ForeColor = settings.DarkModeOffset2TextColor;
                    this.bottom2.ForeColor = settings.DarkModeOffset2TextColor;
                    this.top3.ForeColor = settings.DarkModeOffset3TextColor;
                    this.bottom3.ForeColor = settings.DarkModeOffset3TextColor;
                } else {
                    this.BackColor = settings.LightModeBackgroundColor;
                    this.selected.ForeColor = settings.LightModeSelectedTextColor;
                    this.top1.ForeColor = settings.LightModeOffset1TextColor;
                    this.bottom1.ForeColor = settings.LightModeOffset1TextColor;
                    this.top2.ForeColor = settings.LightModeOffset2TextColor;
                    this.bottom2.ForeColor = settings.LightModeOffset2TextColor;
                    this.top3.ForeColor = settings.LightModeOffset3TextColor;
                    this.bottom3.ForeColor = settings.LightModeOffset3TextColor;
                }
            }
            
        }

        private void WheelSelect_KeyPress(object sender, KeyPressEventArgs e)
        {
            inputQueue += e.KeyChar.ToString();
        }

        private void TryFindMatch(string matchThis)
        {
            var hits = theList.Where(x => x.ToLower().Contains(matchThis.ToLower()));
            if (hits.Count() >= 1) {
                var matchNum = FindFirstMatchIndex(hits.First());
                ScrollToIndex(matchNum);
            }
        }

        private int FindFirstMatchIndex(string str)
        {
            var index = 0;
            foreach (var item in theList) {
                if (item == str) {
                    return index;
                }
                index++;
            }
            return 0;
        }

        private void ScrollToIndex(int newIndex)
        {
            var diff = newIndex - selectedIndex;
            var diffAbs = Math.Abs(diff);
            var scrollUp = true;
            
            // the target value is further down the list and we need to scroll down to get to it
            if (diff > 0) {
                scrollUp = false;
            }
            for (var i = 0; i < diffAbs; i++) {
                HandleEvent(scrollUp);
            }
        }
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
