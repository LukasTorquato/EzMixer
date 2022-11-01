using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

        RegistryKey RegistryKey = Registry.CurrentUser.OpenSubKey(Constants.RegistryKeyPath, true);

        public PreferencesView(MainWindow w)
        {
            InitializeComponent();
            MWindow = w;
            Preferences = new Dictionary<string, string>();
            LoadState();
        }

        private void LoadState() 
        {
            if (File.Exists(Constants.PrefFileLocation)) // Maybe this can go to LoadConfig() in MainWindow
            {
                // Reading saved values from the file
                string jsonString = File.ReadAllText(Constants.PrefFileLocation);
                Preferences = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ?? throw new ArgumentException();

                if (Preferences[Constants.ExitOnCloseKey] != null)
                    EOC_Toggle.IsOn = bool.Parse(Preferences[Constants.ExitOnCloseKey]);
                if (Preferences[Constants.WindowsStartupKey] != null)
                    WinStartup_Toggle.IsOn = bool.Parse(Preferences[Constants.WindowsStartupKey]);
                if (Preferences[Constants.StartMinimizedKey] != null)
                    StartMinimized_Toggle.IsOn = bool.Parse(Preferences[Constants.StartMinimizedKey]);
                if (Preferences[Constants.PollingRateKey] != null)
                    PollingRate_Slider.Value = int.Parse(Preferences[Constants.PollingRateKey]);
                if (Preferences[Constants.SensibilityKey] != null)
                    Sensibility_Slider.Value = int.Parse(Preferences[Constants.SensibilityKey]);
            }
            else
            {
                // Setting up stock values and creating a new file
                Preferences[Constants.ExitOnCloseKey] = false.ToString();
                Preferences[Constants.WindowsStartupKey] = false.ToString();
                Preferences[Constants.StartMinimizedKey] = false.ToString();
                Preferences[Constants.PollingRateKey] = 2.ToString();
                Preferences[Constants.SensibilityKey] = 1.ToString();
                SaveState();
            }
        }

        // Save all flags and controls to the preferences.json file
        private void SaveState()
        {
            string jsonString = JsonSerializer.Serialize(Preferences);
            File.WriteAllText(Constants.PrefFileLocation, jsonString);
        }

        // Change the flag that determines if the application is exited on close
        private void CloseToTray_Toggled(object sender, RoutedEventArgs e)
        {
            MWindow.exitOnClose = EOC_Toggle.IsOn;
            Preferences[Constants.ExitOnCloseKey] = EOC_Toggle.IsOn.ToString();
            SaveState();
        }

        // Change polling rate by changing the value of delay in ms
        private void PollingRate_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ReportRate_Text != null)
            {   
                int value = (int)e.NewValue;
                Preferences[Constants.SensibilityKey] = value.ToString();
                ReportRate_Text.Text = (value * 50).ToString() + " Hz";
                MWindow.pollingMS = 1000 / (value * 50);
                MWindow.Hardware.UpdatePollingRate(value.ToString());
                SaveState();
            }
        }

        private void WinStartup_Toggled(object sender, RoutedEventArgs e)
        {
            if (WinStartup_Toggle.IsOn && RegistryKey.GetValue(@Assembly.GetEntryAssembly().GetName().Name) == null)
            {
                RegistryKey.SetValue(@Assembly.GetEntryAssembly().GetName().Name, Assembly.GetEntryAssembly());
            }
            else if (!WinStartup_Toggle.IsOn && RegistryKey.GetValue(@Assembly.GetEntryAssembly().GetName().Name) != null)
            {
                RegistryKey.DeleteValue(@Assembly.GetEntryAssembly().GetName().Name);
            }

            Preferences[Constants.WindowsStartupKey] = WinStartup_Toggle.IsOn.ToString();
            SaveState();
        }

        // Change sensibility variation
        private void Sensibility_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Sensibility_Text != null)
            {
                int value = (int)e.NewValue;
                Preferences[Constants.SensibilityKey] = value.ToString();
                Sensibility_Text.Text = value.ToString();
                MWindow.sensibility = value;
                SaveState();
            }

        }

        private void StartMinimized_Toggled(object sender, RoutedEventArgs e)
        {
            MWindow.startMinimized = StartMinimized_Toggle.IsOn;
            Preferences[Constants.StartMinimizedKey] = StartMinimized_Toggle.IsOn.ToString();
            SaveState();
        }
    }
}
