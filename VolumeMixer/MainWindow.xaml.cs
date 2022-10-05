using VolumeMixer.Views;
using System.Windows;
using MahApps.Metro.Controls;

namespace VolumeMixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private readonly MainView MView;

        private readonly LightingView LView;

        private readonly PreferencesView PView;


        public MainWindow()
        {
            InitializeComponent();
            MView = new MainView();
            LView = new LightingView(MView.Controller,MView.Hardware,MView.numSliders);
            PView = new PreferencesView();
            this.MainContentControl.Content = MView;

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
