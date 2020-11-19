using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WindowsDesktop;
using System.Drawing;

namespace VirtualDesktopManager
{
    public partial class Form1 : Form
    {
        // [DllImport("user32.dll", ExactSpelling = true)]
        // static extern IntPtr GetForegroundWindow();

        // [DllImport("user32.dll")]
        // [return: MarshalAs(UnmanagedType.Bool)]
        // static extern bool SetForegroundWindow(IntPtr hWnd);

        private IList<VirtualDesktop> _desktops;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        private bool _closeToTray;

        public Form1()
        {
            InitializeComponent();

            HandleChangedNumber();

            _closeToTray = true;
            
            VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
            VirtualDesktop.Created += VirtualDesktop_Added;
            VirtualDesktop.Destroyed += VirtualDesktop_Destroyed;

            FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_closeToTray)
                return;
                    
            e.Cancel = true;
            Visible = false;
            ShowInTaskbar = false;
            notifyIcon1.BalloonTipTitle = "Still Running...";
            notifyIcon1.BalloonTipText = "Right-click on the tray icon to exit.";
            notifyIcon1.ShowBalloonTip(2000);
        }

        private void HandleChangedNumber()
        {
            _desktops = VirtualDesktop.GetDesktops();
        }

        private void VirtualDesktop_Added(object sender, VirtualDesktop e) => HandleChangedNumber();

        private void VirtualDesktop_Destroyed(object sender, VirtualDesktopDestroyEventArgs e) => HandleChangedNumber();

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            // 0 == first
            var currentDesktopIndex = GetCurrentDesktopIndex();
            ChangeTrayIcon(currentDesktopIndex);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _closeToTray = false;
                Close();
            }
            catch (Exception ex)
            {
                var ee = ex.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";

            InitialDesktopState();
            ChangeTrayIcon();

            Visible = false;
        }

        private int GetCurrentDesktopIndex()
        {
            return _desktops.IndexOf(VirtualDesktop.Current);
        }

        private void ChangeTrayIcon(int currentDesktopIndex = -1)
        {
            if(currentDesktopIndex == -1) 
                currentDesktopIndex = GetCurrentDesktopIndex();

            var desktopNumber = currentDesktopIndex + 1;

            // var fontSize = 180;
            // var xPlacement = 50;
            // var yPlacement = 0;

            // var fontSize = 220;
            // var xPlacement = 50;
            // var yPlacement = -20;

            var fontSize = 240;
            var xPlacement = 38;
            var yPlacement = -40;

            if(desktopNumber > 9 && desktopNumber < 100)
            {
                fontSize = 125;
                xPlacement = 75;
                yPlacement = 65;
            } else if(desktopNumber > 99)
            {
                fontSize = 80;
                xPlacement = 90;
                yPlacement = 100;
            }

            Bitmap newIcon = Properties.Resources.mainIcoPng;
            Font desktopNumberFont = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

            using (var gr = Graphics.FromImage(newIcon))
            {
                gr.DrawString(desktopNumber.ToString(), desktopNumberFont, Brushes.White, xPlacement, yPlacement);

                Icon numberedIcon = Icon.FromHandle(newIcon.GetHicon());
                notifyIcon1.Icon = numberedIcon;

                DestroyIcon(numberedIcon.Handle);
                desktopNumberFont.Dispose();
                newIcon.Dispose();
            }
        }

        private VirtualDesktop InitialDesktopState()
        {
            return VirtualDesktop.Current;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
        }
    }
}
