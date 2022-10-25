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

namespace EzMixer.Views
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

        public GroupView(MainWindow w, Mixer c)
        {
            InitializeComponent();
            Controller = c;
            window = w;
            AvailablePrograms = new List<string>();
            UpdateListView();

            GroupCombo.ItemsSource = Controller.Groups.Keys.ToList();

        }

        private static bool IsGroup(string s)
        {
            if(s.StartsWith(Constants.GroupHeader))
                return true;
            else if (string.Equals(s, Constants.Master))
                return true;
            else if (string.Equals(s, Constants.Mic))
                return true;
            
            return false;
        }

        private void SaveState()
        {
            string jsonString = JsonSerializer.Serialize(Controller.Groups);
            File.WriteAllText(Constants.GroupsFileLocation, jsonString);
        }

        private void UpdateGroupSessions(string groupName, bool delete=false)
        {
            groupName = Constants.GroupHeader + groupName;

            int pos = Array.FindIndex(Controller.GetState()[Constants.StateKeys], row => row == groupName);

            if (pos > 0)
            {
                Controller.UpdateAudioSession(pos, groupName, delete);
                if (delete)
                {
                    Controller.GetState()[Constants.StateKeys][pos] = null;
                    Controller.GetState()[Constants.StateNames][pos] = null;
                    window.UpdateMainViewCombo();
                }
            }
        }

        public void UpdateListView()
        {
            AvailablePrograms = Controller.AvailableApps.Values.ToList();
            AvailablePrograms.RemoveAll(IsGroup);
            Available_ListView.ItemsSource = AvailablePrograms.ToList();
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                if (groupName != null)
                {
                    Selected_ListView.ItemsSource = Controller.Groups[groupName].ToList();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error on Group_SelectionChanged: "+ex.ToString());
            }
        }

        //Função para criar um grupo
        private async void CreateGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = await window.ShowInputAsync("Create Group", "Enter a new name for the group:");

            if (name != null)
            {
                if (Controller.Groups.ContainsKey(name))
                    await window.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                else
                {
                    Controller.Groups[name] = new List<string>();
                    Controller.AvailableApps[Constants.GroupHeader + name] = Constants.GroupHeader + name;
                    GroupCombo.ItemsSource = Controller.Groups.Keys.ToList();
                    window.UpdateMainViewCombo();
                    SaveState();
                }
            }
            else
                await window.ShowMessageAsync("Invalid Name", "No input detected.");
        }

        //Função para renomear um grupo
        private async void RenameGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = await window.ShowInputAsync("Rename Group", "Enter a new name for the group:");
            string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();

            if (name != null)
            {
                if (Controller.Groups.ContainsKey(name))
                    await window.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                else
                {
                    Controller.Groups[name] = Controller.Groups[groupName];
                    //Removing old
                    Controller.Groups.Remove(groupName);
                    Controller.AvailableApps.Remove(Constants.GroupHeader + groupName);

                    //Adding new
                    GroupCombo.ItemsSource = Controller.Groups.Keys.ToList();
                    GroupCombo.SelectedItem = name;
                    Controller.AvailableApps[Constants.GroupHeader + name] = Constants.GroupHeader + name;

                    int pos = Array.FindIndex(Controller.GetState()[Constants.StateKeys], row => row == Constants.GroupHeader+groupName);
                    if (pos > 0)
                    {
                        Controller.GetState()[Constants.StateKeys][pos] = Constants.GroupHeader + name;
                        Controller.GetState()[Constants.StateNames][pos] = Constants.GroupHeader + name;
                        window.UpdateMainViewCombo();
                    }
                    SaveState();
                }
            }
        }

        //Função para deletar um grupo
        private async void DeleteGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = await window.ShowMessageAsync("Deletar", "Deseja realmente deletar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative);
            string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();

            if (name.ToString() == "Affirmative" && Controller.Groups.ContainsKey(groupName))
            {
                Controller.Groups.Remove(groupName);
                Controller.AvailableApps.Remove(Constants.GroupHeader + groupName);
                Selected_ListView.ItemsSource = null;
                GroupCombo.ItemsSource = Controller.Groups.Keys.ToList();
                UpdateGroupSessions(groupName, true);
                SaveState();
            }
        }
        
        //Função para adicionar uma session do mixer à um grupo
        private void AddProgram_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                string selectedApp = Convert.ToString(Available_ListView.SelectedItem) ?? throw new Exception();
                if (groupName != null && selectedApp != null)
                {
                    Controller.Groups[groupName].Add(selectedApp);
                    Selected_ListView.ItemsSource = Controller.Groups[groupName].ToList();
                    UpdateGroupSessions(groupName);
                    SaveState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        
        //Função para remover uma session do mixer à um grupo
        private void RemoveProgram_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                string selectedApp = Convert.ToString(Selected_ListView.SelectedItem) ?? throw new Exception();
                if (groupName != null && selectedApp != null)
                {
                    Controller.Groups[groupName].Remove(selectedApp);
                    Selected_ListView.ItemsSource = Controller.Groups[groupName].ToList();
                    UpdateGroupSessions(groupName);
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
