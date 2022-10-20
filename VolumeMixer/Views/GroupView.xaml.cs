using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace VolumeMixer.Views
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
    public partial class GroupView : UserControl
    {
        private Mixer Controller;

        private MainWindow window;

        public List<string> AvailablePrograms { get; set; }

        public List<string> SelectedPrograms { get; set; }

        private Dictionary<string, List<string>> Groups { get; set; }

        public GroupView(Mixer c, MainWindow w)
        {
            InitializeComponent();
            Controller = c;
            window = w;
            AvailablePrograms = new List<string>();
            SelectedPrograms = new List<string>();
            Groups = new Dictionary<string, List<string>>();
            LoadState();
            AvailablePrograms = Controller.AvailableApps.Values.ToList();
            
            foreach ( var proc in AvailablePrograms)
            {
                Available_ListView.Items.Add(proc);
            }
            
        }

        private void LoadState()
        {
            if (File.Exists(Constants.GroupsFileLocation))
            {
                string jsonString = File.ReadAllText(Constants.GroupsFileLocation);
                Groups = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString) ?? throw new ArgumentException();
                GroupCombo.ItemsSource = Groups.Keys.ToList();
            }
                
        }

        private void SaveState()
        {
            string jsonString = JsonSerializer.Serialize(Groups);
            File.WriteAllText(Constants.GroupsFileLocation, jsonString);
        }


        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
            if(groupName != null)
            {
                Selected_ListView.Items.Clear();

                foreach (var programName in Groups[groupName])
                {
                    Selected_ListView.Items.Add(programName);
                }
            }
        }

        private async void AddGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = await window.ShowInputAsync("Add Group", "Enter a new name for the group:");
            if (name != null)
            {
                if (Groups.ContainsKey(name))
                    await window.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                else
                {
                    Groups[name] = new List<string>();
                    GroupCombo.Items.Add(name.ToString());
                    SaveState();
                }
            }
            else
                await window.ShowMessageAsync("Invalid Name", "No input detected.");
        }

        private async void RenameGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = await window.ShowInputAsync("Rename Group", "Enter a new name for the group:");
            string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();

            if (name != null)
            {
                if (Groups.ContainsKey(name))
                    await window.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                else
                {
                    Groups[name] = Groups[groupName];
                    Groups.Remove(groupName);
                    GroupCombo.Items.Remove(groupName);
                    GroupCombo.Items.Add(name.ToString());
                    SaveState();
                }
            }

        }

        private void DeleteGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = window.ShowMessageAsync("Deletar", "Deseja realmente deletar?");
            string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
            Debug.WriteLine(name);
            /*
            if (name != null)
            {
                if (Groups.ContainsKey(name))
                    await window.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                else
                {
                    Groups[name] = Groups[groupName];
                    Groups.Remove(groupName);
                    GroupCombo.Items.Remove(groupName);
                    GroupCombo.Items.Add(name.ToString());
                    SaveState();
                }
            }*/
        }


        private void AddProgram_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                string selectedApp = Convert.ToString(Available_ListView.SelectedItem) ?? throw new Exception();
                if (groupName != null && selectedApp != null)
                {
                    Groups[groupName].Add(selectedApp);
                    Selected_ListView.Items.Add(selectedApp);
                    SaveState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void RemoveProgram_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                string selectedApp = Convert.ToString(Selected_ListView.SelectedItem) ?? throw new Exception();
                if (groupName != null && selectedApp != null)
                {
                    Groups[groupName].Remove(selectedApp);
                    Selected_ListView.Items.Remove(selectedApp);
                    SaveState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
