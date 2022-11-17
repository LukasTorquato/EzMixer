using EzMixer.Views;
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
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace EzMixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        // VIEWS
        public readonly MainView MainView;
        public readonly GroupView GroupView;
        public readonly LightingView LightingView;
        public readonly PreferencesView PreferencesView;

        private System.Windows.Forms.NotifyIcon trayIcon;

        // Main Variables
        public readonly int numSliders = 5;
        public int sensibility = 2;
        public int pollingMS = 10;
        private double[] Volumes;

        // Watcher Variables
        public SessionCreatedWatcher SCWatcher;
        public SessionDisconnectedWatcher SDWatcher;
        private DeviceWatcher DeviceWatcher;

        // Mixer and Hardware Variables
        public Mixer Controller { get; set; }
        public SerialDevice Hardware;

        // Config Variables
        public bool exitOnClose = false;
        public bool startMinimized = false;

        // Startup Variables
        public RegistryKey registryKey;


        public MainWindow()
        {
            InitializeComponent();

            // Initialize main variables
            Controller = new Mixer(numSliders);
            Hardware = new SerialDevice(numSliders);
            Volumes = new double[numSliders];

            // Start Watchers
            SCWatcher = new SessionCreatedWatcher(this);
            Controller.SetSessionObserver(SCWatcher);
            SDWatcher = new SessionDisconnectedWatcher(this);
            Controller.SetSessionDObserver(SDWatcher);
            DeviceWatcher = new DeviceWatcher(this);
            Mixer.SetDeviceObserver(DeviceWatcher);

            // Load config files
            LoadConfig();

            // Start Listening Thread
            Thread waitThread = new Thread(new ThreadStart(WaitForDevice));
            waitThread.IsBackground = true;
            waitThread.Start();

            // Initialize views
            MainView = new MainView(this);
            GroupView = new GroupView(this);
            LightingView = new LightingView(this);
            PreferencesView = new PreferencesView(this);

            UpdateMainViewCombos();

            this.MainContentControl.Content = MainView;
            CreateTrayMenu();

            if(startMinimized)
                this.Hide();
        }

        // Function to check if the device is connected 
        private void WaitForDevice()
        {
            while (true)
            {
                if (Hardware.Connected)
                {
                    Hardware.UpdateLighting();
                    Thread updateVolThread = new Thread(new ThreadStart(UpdateVolumes));
                    updateVolThread.IsBackground = true;
                    updateVolThread.Start();
                    break;
                }
                else
                {
                    //Debug.WriteLine("Waiting for device...");
                    Hardware.ScanDevicePort();
                }
                Thread.Sleep(1000);
            }
            //Debug.WriteLine("Found Device...");

        }

        // Function to update all volumes on the mixer
        private void UpdateVolumes()
        {
            double[] old = {0,0,0,0,0};
            double noiseReductionThreshold = 0;
            while (true)
            {
                try
                {
                    Volumes = Hardware.GetVolume(sensibility);
                    for (int i = 0; i < numSliders; i++)
                    {
                        if (Math.Abs(Volumes[i] - old[i]) - sensibility > 0 || Volumes[i] == 0 || Volumes[i] == 100)
                        {
                            old[i] = Volumes[i];
                            Controller.SetSessionVolume(i, Volumes[i]);
                        }
                    }
                    Hardware.ClearInBuffer();
                    Thread.Sleep(pollingMS);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error on UpdateVolume(): " + ex.Message);
                    Thread waitThread = new Thread(new ThreadStart(WaitForDevice));
                    waitThread.IsBackground = true;
                    waitThread.Start();
                    break;
                }
            }
        }

        public void SaveState()
        {
            Dictionary<string, string[]> jsonDictionary = Controller.GetState();
            string jsonString = JsonSerializer.Serialize(jsonDictionary);
            File.WriteAllText(Constants.FileLocation, jsonString);
        }

        private void LoadConfig() // UMV (Utilizavel em Main View)
        {
            try
            {

                if (File.Exists(Constants.GroupsFileLocation))
                {
                    string jsonString = File.ReadAllText(Constants.GroupsFileLocation);
                    Dictionary<string, List<string>> groupsDic = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString) ?? throw new ArgumentException();
                    Controller.Groups = groupsDic;
                }

                if (File.Exists(Constants.FileLocation))
                {
                    string jsonString = File.ReadAllText(Constants.FileLocation);
                    Dictionary<string, string[]> jsonDic = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonString) ?? throw new ArgumentException();

                    Controller.LoadState(jsonDic);
                }
                else
                    Controller.LoadState();


                SaveState();
                //update lighting at start
                string message = "a5";
                for (int i = 0; i < numSliders; i++)
                {
                    message += i + "=" + Controller.GetState()[Constants.StateLighting][i] + "|";
                }
                Hardware.LightingCommand = message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on LoadConfig(): " + ex.Message);
            }
        }

        public void UpdateMainViewCombos()
        {
            MainView.ComboLoad();
            MainView.ComboRefresh();
        }

        private void CreateTrayMenu()
        {   
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.DoubleClick += (s, args) => ShowMainWindow();
            trayIcon.Icon = EzMixer.Properties.Resources.mainicon;
            trayIcon.Visible = true;

            trayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Open Settings").Click += (s, e) => ShowMainWindow();
            trayIcon.ContextMenuStrip.Items.Add("Reload Sessions").Click += (s, e) => Controller.ReloadAudioSessions();
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
            if (MainView is null || LightingView is null || PreferencesView is null)
            {
                return;
            }
            if (lightingRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Lighting Selection";
                this.MainContentControl.Content = LightingView;
            }
            else if (groupRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Groups Settings";
                this.MainContentControl.Content = GroupView;
            }
            else if (homeRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Application Selection";
                this.MainContentControl.Content = MainView;
            }
            else if (preferencesRadio.IsChecked == true)
            {
                this.ViewTitle.Text = "Configurations";
                this.MainContentControl.Content = PreferencesView;
            }
        }

        private void ReloadSessions_Click(object sender, RoutedEventArgs e) => Controller.ReloadAudioSessions();
    }
}
