using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace EzMixer.Views
{
    /// <summary>
    /// Interaction logic for PreferencesView.xaml
    /// </summary>

    public partial class PreferencesView : UserControl
    {

        private Dictionary<string, string> Preferences;

        private MainWindow MWindow;

        private bool start = true;
        
        public PreferencesView(MainWindow w)
        {
            InitializeComponent();
            MWindow = w;
            Preferences = new Dictionary<string, string>();
            start = false;
            LoadState();
        }

        private void LoadState()
        {
            if (File.Exists(Constants.PrefFileLocation))
            {
                string jsonString = File.ReadAllText(Constants.PrefFileLocation);
                Preferences = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ?? throw new ArgumentException();

                if (Preferences[Constants.ExitOnCloseKey] != null)
                    EOC_Toggle.IsOn = bool.Parse(Preferences[Constants.ExitOnCloseKey]);
                if (Preferences[Constants.WindowsStartupKey] != null)
                    WinStartup_Toggle.IsOn = bool.Parse(Preferences[Constants.WindowsStartupKey]);
                if (Preferences[Constants.PollingRateKey] != null)
                    PollingRate_Slider.Value = int.Parse(Preferences[Constants.PollingRateKey]);
                if (Preferences[Constants.SensibilityKey] != null)
                    Sensibility_Slider.Value = int.Parse(Preferences[Constants.SensibilityKey]);
            }
            else
            {
                Preferences[Constants.ExitOnCloseKey] = false.ToString();
                Preferences[Constants.WindowsStartupKey] = false.ToString();
                Preferences[Constants.PollingRateKey] = 2.ToString();
                Preferences[Constants.SensibilityKey] = 1.ToString();
                SaveState();
            }
        }

        private void SaveState()
        {
            string jsonString = JsonSerializer.Serialize(Preferences);
            File.WriteAllText(Constants.PrefFileLocation, jsonString);
        }

        private void CloseToTray_Toggled(object sender, RoutedEventArgs e)
        {
            MWindow.exitOnClose = EOC_Toggle.IsOn;
            Preferences[Constants.ExitOnCloseKey] = EOC_Toggle.IsOn.ToString();
            SaveState();
        }

        private void PollingRate_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (start)
                return;

            int value = (int)e.NewValue;
            if (ReportRate_Text != null)
            {
                Preferences[Constants.SensibilityKey] = value.ToString();
                ReportRate_Text.Text = (value * 50).ToString() + " Hz";
                MWindow.Hardware.UpdatePollingRate(value.ToString());
                MWindow.pollingMS = 1000 / (value * 50);
                SaveState();
            }

        }

        private void WinStartup_Toggled(object sender, RoutedEventArgs e)
        {
            /*if (WinStartup_Toggle.IsOn == true)
                window.exitOnClose = true;
            else
                window.exitOnClose = false;*/

            Preferences[Constants.WindowsStartupKey] = WinStartup_Toggle.IsOn.ToString();
            SaveState();
        }

        private void Sensibility_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (start)
                return;

            int value = (int)e.NewValue;
            if (Sensibility_Text != null)
            {
                Preferences[Constants.SensibilityKey] = value.ToString();
                Sensibility_Text.Text = value.ToString();
                MWindow.sensibility = value;
                SaveState();
            }

        }
    }
}
