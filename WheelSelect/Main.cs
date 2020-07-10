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
        private DateTime lastInputTime = DateTime.Now;
        private string outputLocation = ConfigurationManager.AppSettings["OutputLocation"] ?? @"c:\wheel_selection.txt";
        private string clearOutputFileOnStart = ConfigurationManager.AppSettings["ClearOutputFileOnStart"];
        private string clearOutputFileOnEscape = ConfigurationManager.AppSettings["ClearOutputFileOnEscape"];
        private string outputMethod = ConfigurationManager.AppSettings["OutputMethod"];
        private string windowBackgroundColor = ConfigurationManager.AppSettings["WindowBackgroundColor"];
        private string selectedTextColor = ConfigurationManager.AppSettings["SelectedTextColor"];
        private string offset1TextColor = ConfigurationManager.AppSettings["Offset1TextColor"];
        private string offset2TextColor = ConfigurationManager.AppSettings["Offset2TextColor"];
        private string offset3TextColor = ConfigurationManager.AppSettings["Offset3TextColor"];
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
            bool clearOnStart;
            if (bool.TryParse(clearOutputFileOnStart, out clearOnStart)) {
                if (clearOnStart) {
                    WriteToSaveFile("");
                }
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
            if (e.KeyCode == Keys.Escape) {
                bool clearOnEsc;
                if (bool.TryParse(clearOutputFileOnEscape, out clearOnEsc)) {
                    if (clearOnEsc) {
                        WriteToSaveFile("");
                    }
                }
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
            WriteToSaveFile(selected.Text);
            Environment.Exit(0);
        }

        private void WriteToSaveFile(string value)
        {
            OutputMethod om;
            if (Enum.TryParse(outputMethod, out om)) {
                switch (om) {
                    case OutputMethod.Overwrite:
                        File.WriteAllText(outputLocation, value);
                        break;
                    case OutputMethod.Append:
                        File.AppendAllText(outputLocation, value);
                        break;
                    default:
                        File.WriteAllText(outputLocation, value);
                        break;
                }
            }
            
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

        private void WheelSelect_KeyPress(object sender, KeyPressEventArgs e)
        {
            lastInputTime = DateTime.Now;
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

    public enum OutputMethod
    {
        Overwrite,
        Append
    }
}
