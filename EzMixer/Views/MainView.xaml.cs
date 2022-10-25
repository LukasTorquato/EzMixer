using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace EzMixer.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {

        public readonly int numSliders = 5;

        private int sensibility = 2;

        private int pollingMS = 10;

        private double[] Volumes;

        public SessionCreatedWatcher SCWatcher;

        public SessionDisconnectedWatcher SDWatcher;

        private DeviceWatcher DeviceWatcher;

        public Mixer Controller { get; set; }
        
        public SerialDevice Hardware;

        public MainView()
        {
            InitializeComponent();

            DataContext = this;

            Controller = new Mixer(numSliders);
            Hardware = new SerialDevice(numSliders);
            Volumes = new double[numSliders];

            SCWatcher = new SessionCreatedWatcher(this);
            Controller.SetSessionObserver(SCWatcher);
            SDWatcher = new SessionDisconnectedWatcher(this);
            Controller.SetSessionDObserver(SDWatcher);
            DeviceWatcher = new DeviceWatcher(this);
            Mixer.SetDeviceObserver(DeviceWatcher);
            LoadConfig();

            Thread waitThread = new Thread(new ThreadStart(WaitForDevice));
            waitThread.IsBackground = true;
            waitThread.Start();
        }

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

        private void SaveState()
        {
            Dictionary<string, string[]> jsonDictionary = Controller.GetState();
            string jsonString = JsonSerializer.Serialize(jsonDictionary);
            File.WriteAllText(Constants.FileLocation, jsonString);
        }

        public void LoadConfig()
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
                    
                    string message = "a5";
                    for (int i = 0; i < numSliders; i++)
                    {
                        if (jsonDic[Constants.StateLighting][i] == null)
                            jsonDic[Constants.StateLighting][i] = "FFFFFF*";
                        
                        message += i + "=" + jsonDic[Constants.StateLighting][i] + "|";
                    }
                    Controller.LoadState(jsonDic);
                    Hardware.LightingCommand = message;
                    //ComboLoad(jsonDic[Constants.StateNames]);
                    ComboLoad();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on LoadConfig(): " + ex.Message);
            }
        }

        public void ComboLoad()
        {
            this.Dispatcher.Invoke(() =>
            {
                volume1.SelectedItem = Controller.GetState()[Constants.StateNames][0];
                volume2.SelectedItem = Controller.GetState()[Constants.StateNames][1];
                volume3.SelectedItem = Controller.GetState()[Constants.StateNames][2];
                volume4.SelectedItem = Controller.GetState()[Constants.StateNames][3];
                volume5.SelectedItem = Controller.GetState()[Constants.StateNames][4];
            });
        }

        public void ComboRefresh()
        {
            this.Dispatcher.Invoke(() =>
            {
                volume1.ItemsSource = Controller.AvailableApps.Values.ToList();
                volume2.ItemsSource = Controller.AvailableApps.Values.ToList();
                volume3.ItemsSource = Controller.AvailableApps.Values.ToList();
                volume4.ItemsSource = Controller.AvailableApps.Values.ToList();
                volume5.ItemsSource = Controller.AvailableApps.Values.ToList();
            });
        }

        private void Session_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Source == volume1 && volume1.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume1.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(0, appName);
                } 
                else if (e.Source == volume2 && volume2.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume2.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(1, appName);
                }
                else if (e.Source == volume3 && volume3.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume3.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(2, appName);
                }
                else if (e.Source == volume4 && volume4.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume4.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(3, appName);
                }
                else if (e.Source == volume5 && volume5.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume5.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(4, appName);
                }

                SaveState();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
