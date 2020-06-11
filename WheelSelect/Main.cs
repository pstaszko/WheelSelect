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
        private string saveLocation = ConfigurationManager.AppSettings["SaveLocation"] ?? @"c:\wheel_selection.txt";
        private string windowBackgroundColor = ConfigurationManager.AppSettings["WindowBackgroundColor"];
        private string selectedTextColor = ConfigurationManager.AppSettings["SelectedTextColor"];
        private string offset1TextColor = ConfigurationManager.AppSettings["Offset1TextColor"];
        private string offset2TextColor = ConfigurationManager.AppSettings["Offset2TextColor"];
        private string offset3TextColor = ConfigurationManager.AppSettings["Offset3TextColor"];

        public WheelSelect()
        {
            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TrySetColors();
            HandleArgs();
            HandleEvent(false);
        }

        private void HandleArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            // args[0] seems to always = execution path. Neat.
            if (args.Length == 3) {
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
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                Environment.Exit(0);
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
            File.AppendAllText(saveLocation, selected.Text);
            Environment.Exit(0);
        }

        private void TrySetColors()
        {
            try {
                this.BackColor = Color.FromName(windowBackgroundColor);
            } catch (Exception) {
                this.BackColor = Color.White;
            }
            try {
                this.selected.ForeColor = Color.FromName(selectedTextColor);
            } catch (Exception) {
                this.selected.ForeColor = Color.SteelBlue;
            }
            try {
                this.top1.ForeColor = Color.FromName(offset1TextColor);
                this.bottom1.ForeColor = Color.FromName(offset1TextColor);
            } catch (Exception) {
                this.top1.ForeColor = Color.DimGray;
                this.bottom1.ForeColor = Color.DimGray;
            }
            try {
                this.top2.ForeColor = Color.FromName(offset2TextColor);
                this.bottom2.ForeColor = Color.FromName(offset2TextColor);
            } catch (Exception) {
                this.top2.ForeColor = Color.Gray;
                this.bottom2.ForeColor = Color.Gray;
            }
            try {
                this.top3.ForeColor = Color.FromName(offset3TextColor);
                this.bottom3.ForeColor = Color.FromName(offset3TextColor);
            } catch (Exception) {
                this.top3.ForeColor = Color.Silver;
                this.bottom3.ForeColor = Color.Silver;
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
}
