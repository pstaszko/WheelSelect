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
        private Settings settings;
        private List<string> filteredList = new List<string>();
        private int selectedIndex = 0;

        public WheelSelect()
        {
            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;
            settings = new Settings().GetSettings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (settings.ClearOutputFileOnStart) {
                try {
                    WriteToSaveFile("");
                } catch (Exception) { }
            }
            TrySetColors();
            selectedIndex = -1;
            filteredList.AddRange(settings.Data);
            HandleEvent(false);
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
            if (trackInt > filteredList.Count() - 1) {
                return "";
            }
            if (trackInt < 0) {
                return "";
            }
            return filteredList[trackInt];
        }

        private int GetPreviousIndex()
        {
            if (selectedIndex == 0) {
                return 0;
            } else if (selectedIndex == filteredList.Count()) {
                return selectedIndex;
            }
            var returnVal = selectedIndex - 1;
            return returnVal;
        }

        private int GetNextIndex()
        {
            if (selectedIndex == filteredList.Count() - 1) {
                return selectedIndex;
            }
            var returnVal = selectedIndex + 1;
            return returnVal;
        }

        private void SaveSelection()
        {
            try {
                WriteToSaveFile(selected.Text);
            } catch (Exception exn) {
                MessageBox.Show(String.Format("Could not write to output location:\n[{0}].\n\nSpecifically:\n{1}", settings.OutputLocation, exn.Message)
                    , "Error"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
            }
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

        private void SetDarkMode()
        {
            this.txtInput.BackColor = settings.DarkModeBackgroundColor;
            this.txtInput.ForeColor = settings.DarkModeInputTextColor;
            this.BackColor = settings.DarkModeBackgroundColor;
            this.selected.ForeColor = settings.DarkModeSelectedTextColor;
            this.top1.ForeColor = settings.DarkModeOffset1TextColor;
            this.bottom1.ForeColor = settings.DarkModeOffset1TextColor;
            this.top2.ForeColor = settings.DarkModeOffset2TextColor;
            this.bottom2.ForeColor = settings.DarkModeOffset2TextColor;
            this.top3.ForeColor = settings.DarkModeOffset3TextColor;
            this.bottom3.ForeColor = settings.DarkModeOffset3TextColor;
        }

        private void SetLightMode()
        {
            this.txtInput.BackColor = settings.LightModeBackgroundColor;
            this.txtInput.ForeColor = settings.LightModeInputTextColor;
            this.BackColor = settings.LightModeBackgroundColor;
            this.selected.ForeColor = settings.LightModeSelectedTextColor;
            this.top1.ForeColor = settings.LightModeOffset1TextColor;
            this.bottom1.ForeColor = settings.LightModeOffset1TextColor;
            this.top2.ForeColor = settings.LightModeOffset2TextColor;
            this.bottom2.ForeColor = settings.LightModeOffset2TextColor;
            this.top3.ForeColor = settings.LightModeOffset3TextColor;
            this.bottom3.ForeColor = settings.LightModeOffset3TextColor;
        }

        private void TrySetColors()
        {
            if (settings.SyncWithWindowsTheme) {
                var theme = GetWindowsThemeSetting();
                switch (theme) {
                    case WindowsTheme.LightMode:
                        SetLightMode();
                        break;
                    case WindowsTheme.DarkMode:
                        SetDarkMode();
                        break;
                    default:
                        SetLightMode();
                        break;
                }
            } else {
                if (settings.DefaultTheme == WindowsTheme.DarkMode) {
                    SetDarkMode();
                } else {
                    SetLightMode();
                }
            }

        }

        private void TryFindMatch(string matchThis)
        {
            filteredList.Clear();
            filteredList.AddRange(settings.Data.Where(x => x.ToLower().Contains(matchThis.ToLower())).ToList());
            ScrollToIndex(0);
            HandleEvent(true);
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

        private void resetList()
        {
            selectedIndex = 0;
            HandleEvent(true);
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            filteredList.Clear();
            if (txtInput.Text.Length > 0) {
                TryFindMatch(txtInput.Text);
            } else {
                filteredList.AddRange(settings.Data);
                resetList();
            }
        }

        private void txtInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                if (settings.ClearOutputFileOnEscape) {
                    try {
                        WriteToSaveFile("");
                    } catch (Exception) { }
                }
                Application.Exit();
            } else if (e.KeyCode == Keys.Enter) {
                SaveSelection();
            } else if (e.KeyCode == Keys.Up) {
                HandleEvent(true);
                SendKeys.Send("{END}");
            } else if (e.KeyCode == Keys.Down) {
                HandleEvent(false);
                SendKeys.Send("{END}");
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            var theme = GetWindowsThemeSetting();
            switch (theme) {
                case WindowsTheme.LightMode:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle, settings.LightModeBorderColor, ButtonBorderStyle.Solid);
                    break;
                case WindowsTheme.DarkMode:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle, settings.DarkModeBorderColor, ButtonBorderStyle.Solid);
                    break;
                default:
                    break;
            }
        }
    }
}