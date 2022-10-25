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

        private MainWindow MWindow;

        public MainView(MainWindow w)
        {
            InitializeComponent();

            DataContext = this;

            MWindow = w;

        }        

        public void ComboLoad()
        {
            this.Dispatcher.Invoke(() =>
            {
                volume1.SelectedItem = MWindow.Controller.GetState()[Constants.StateNames][0];
                volume2.SelectedItem = MWindow.Controller.GetState()[Constants.StateNames][1];
                volume3.SelectedItem = MWindow.Controller.GetState()[Constants.StateNames][2];
                volume4.SelectedItem = MWindow.Controller.GetState()[Constants.StateNames][3];
                volume5.SelectedItem = MWindow.Controller.GetState()[Constants.StateNames][4];
            });
        }

        public void ComboRefresh()
        {
            this.Dispatcher.Invoke(() =>
            {
                volume1.ItemsSource = MWindow.Controller.AvailableApps.Values.ToList();
                volume2.ItemsSource = MWindow.Controller.AvailableApps.Values.ToList();
                volume3.ItemsSource = MWindow.Controller.AvailableApps.Values.ToList();
                volume4.ItemsSource = MWindow.Controller.AvailableApps.Values.ToList();
                volume5.ItemsSource = MWindow.Controller.AvailableApps.Values.ToList();
            });
        }

        private void Session_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Source == volume1 && volume1.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume1.SelectedItem) ?? throw new Exception();
                    MWindow.Controller.UpdateAudioSession(0, appName);
                } 
                else if (e.Source == volume2 && volume2.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume2.SelectedItem) ?? throw new Exception();
                    MWindow.Controller.UpdateAudioSession(1, appName);
                }
                else if (e.Source == volume3 && volume3.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume3.SelectedItem) ?? throw new Exception();
                    MWindow.Controller.UpdateAudioSession(2, appName);
                }
                else if (e.Source == volume4 && volume4.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume4.SelectedItem) ?? throw new Exception();
                    MWindow.Controller.UpdateAudioSession(3, appName);
                }
                else if (e.Source == volume5 && volume5.SelectedItem != null)
                {
                    string appName = Convert.ToString(volume5.SelectedItem) ?? throw new Exception();
                    MWindow.Controller.UpdateAudioSession(4, appName);
                }

                MWindow.SaveState();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
