using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace VolumeMixer.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {

        private readonly string filename = "../../../../Config/config.json";

        private readonly int numSliders = 5;

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
            LoadState();

            Thread thread = new Thread(new ThreadStart(WaitForDevice));
            thread.Start();
        }

        private void WaitForDevice()
        {
            while (true)
            {
                if (Hardware.Connected)
                {
                    Thread thread = new Thread(new ThreadStart(UpdateVolumes));
                    thread.Start();
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
                    Thread thread = new Thread(new ThreadStart(WaitForDevice));
                    thread.Start();
                    break;
                }
            }
        }

        private void SaveState()
        {
            Dictionary<string, string[]> jsonDictionary = Controller.GetState();
            string jsonString = JsonSerializer.Serialize(jsonDictionary);
            File.WriteAllText(filename, jsonString);
        }

        public void LoadState()
        {
            try
            {
                if (File.Exists(filename))
                {
                    string jsonString = File.ReadAllText(filename);
                    Dictionary<string, string[]> jsonDic = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonString) ?? throw new ArgumentException();
                    Controller.LoadState(jsonDic);

                    ComboLoad(jsonDic[Constants.StateNames]);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        public void ComboLoad(string[] selectedAppNames)
        {
            this.Dispatcher.Invoke(() =>
            {
                volume1.SelectedItem = selectedAppNames[0];
                volume2.SelectedItem = selectedAppNames[1];
                volume3.SelectedItem = selectedAppNames[2];
                volume4.SelectedItem = selectedAppNames[3];
                volume5.SelectedItem = selectedAppNames[4];
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

        private void Volume1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!(volume1.SelectedItem is null))
                {
                    string appName = Convert.ToString(volume1.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(0, appName);
                }

                SaveState();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void Volume2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!(volume2.SelectedItem is null))
                {
                    string appName = Convert.ToString(volume2.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(1, appName);

                }

                SaveState();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Volume3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!(volume3.SelectedItem is null))
                {
                    string appName = Convert.ToString(volume3.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(2, appName);

                }

                SaveState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Volume4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!(volume4.SelectedItem is null))
                {
                    string appName = Convert.ToString(volume4.SelectedItem) ?? throw new Exception();
                    Controller.UpdateAudioSession(3, appName);

                }

                SaveState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Volume5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!(volume5.SelectedItem is null))
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
