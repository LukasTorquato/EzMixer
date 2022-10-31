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

namespace EzMixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        // VIEWS
        public readonly MainView MView;

        public readonly GroupView GView;

        public readonly LightingView LView;

        public readonly PreferencesView PView;

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
            MView = new MainView(this);
            GView = new GroupView(this);
            LView = new LightingView(this);
            PView = new PreferencesView(this);

            UpdateMainViewCombos();

            this.MainContentControl.Content = MView;
            CreateTrayMenu();
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
            double delta = 0;

            while (true)
            {
                try
                {
                    Volumes = Hardware.GetVolume(sensibility);
                    for (int i = 0; i < numSliders; i++)
                    {
                        if (delta - sensibility > Volumes[i] || delta + sensibility < Volumes[i])
                            delta = Volumes[i];
                        Controller.SetSessionVolume(i, Volumes[i]);
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
                    
                    //setting stock lighting
                    string message = "a5";
                    for (int i = 0; i < numSliders; i++)
                    {
                        if (jsonDic[Constants.StateLighting][i] == null)
                            jsonDic[Constants.StateLighting][i] = "FFFFFF*";

                        message += i + "=" + jsonDic[Constants.StateLighting][i] + "|";
                    }
                    Controller.LoadState(jsonDic);
                    Hardware.LightingCommand = message;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on LoadConfig(): " + ex.Message);
            }
        }

        public void UpdateMainViewCombos()
        {
            MView.ComboLoad();
            MView.ComboRefresh();
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

        private void ReloadSessions_Click(object sender, RoutedEventArgs e) => Controller.ReloadAudioSessions();
    }
}
