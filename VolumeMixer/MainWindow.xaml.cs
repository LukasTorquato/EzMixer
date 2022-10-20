using VolumeMixer.Views;
using System.Windows;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Forms;

namespace VolumeMixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private readonly MainView MView;

        private readonly GroupView GView;

        private readonly LightingView LView;

        private readonly PreferencesView PView;

        private System.Windows.Forms.NotifyIcon trayIcon;

        public bool exitOnClose = false;


        public MainWindow()
        {
            InitializeComponent();
            MView = new MainView();
            GView = new GroupView(MView.Controller,this);
            LView = new LightingView(MView.Controller,MView.Hardware,MView.numSliders);
            PView = new PreferencesView(this);
            this.MainContentControl.Content = MView;

            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.DoubleClick += (s, args) => ShowMainWindow();
            trayIcon.Icon = VolumeMixer.Properties.Resources.mainicon;
            trayIcon.Visible = true;

            CreateContextMenu();
        }

        /*public async void StartMessageDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }

        public object StartInputDialog(string title, string message)
        {
            var input = this.ShowInputAsync(title, message);

            return input;
        }*/

        private void CreateContextMenu()
        {
            trayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Open Settings").Click += (s, e) => ShowMainWindow();
            trayIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ShowMainWindow()
        {
            if (this.IsVisible)
            {
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
            else
            {
                this.Show();
            }
        }

        private void ExitApplication()
        {
            exitOnClose = true;
            this.Close();
            trayIcon.Dispose();
            trayIcon = null;
            Environment.Exit(Environment.ExitCode);
        }


        // Função para encerramento do programa
        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            if (!exitOnClose)
                e.Cancel = true;

            this.Hide();
            base.OnClosing(e);
        }


        private void ChangeScreens(object sender, RoutedEventArgs e)
        {
            if (MView is null || LView is null || PView is null)
            {
                return;
            }
            if (lightingRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Lighting Selection";
                this.MainContentControl.Content = LView;
            }
            else if (groupRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Groups Settings";
                this.MainContentControl.Content = GView;
            }
            else if (homeRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Application Selection";
                this.MainContentControl.Content = MView;
            }
            else if (preferencesRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Configurations";
                this.MainContentControl.Content = PView;
            }
        }
    }
}
