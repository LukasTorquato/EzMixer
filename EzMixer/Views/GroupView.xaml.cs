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
        private MainWindow MWindow;

        public List<string> AvailablePrograms { get; set; }

        public List<string> SelectedPrograms { get; set; }

        public GroupView(MainWindow w)
        {
            InitializeComponent();
            MWindow = w;
            AvailablePrograms = new List<string>();
            UpdateListView();
            RenameButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            GroupCombo.ItemsSource = MWindow.Controller.Groups.Keys.ToList();

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
            string jsonString = JsonSerializer.Serialize(MWindow.Controller.Groups);
            File.WriteAllText(Constants.GroupsFileLocation, jsonString);
        }

        private void UpdateGroupSessions(string groupName, bool delete=false)
        {
            groupName = Constants.GroupHeader + groupName;

            int pos = Array.FindIndex(MWindow.Controller.GetState()[Constants.StateKeys], row => row == groupName);

            if (pos > 0)
            {
                MWindow.Controller.UpdateAudioSession(pos, groupName, delete);
                if (delete)
                {
                    MWindow.Controller.GetState()[Constants.StateKeys][pos] = null;
                    MWindow.Controller.GetState()[Constants.StateNames][pos] = null;
                    MWindow.UpdateMainViewCombos();
                }
            }
        }

        public void UpdateListView()
        {
            AvailablePrograms = MWindow.Controller.AvailableApps.Values.ToList();
            AvailablePrograms.RemoveAll(IsGroup);

            this.Dispatcher.Invoke(() =>
            {
                Available_ListView.ItemsSource = AvailablePrograms.ToList();
            });
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                if (groupName != null)
                {
                    Selected_ListView.ItemsSource = MWindow.Controller.Groups[groupName].ToList();
                    RenameButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
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
            var name = await MWindow.ShowInputAsync("Create Group", "Enter a new name for the group:");

            if (name != null)
            {
                if (MWindow.Controller.Groups.ContainsKey(name))
                    await MWindow.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                else
                {
                    MWindow.Controller.Groups[name] = new List<string>();
                    MWindow.Controller.AvailableApps[Constants.GroupHeader + name] = Constants.GroupHeader + name;
                    GroupCombo.ItemsSource = MWindow.Controller.Groups.Keys.ToList();
                    MWindow.UpdateMainViewCombos();
                    SaveState();
                }
            }
            else
                await MWindow.ShowMessageAsync("Invalid Name", "No input detected.");
        }

        //Função para renomear um grupo
        private async void RenameGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var name = await MWindow.ShowInputAsync("Rename Group", "Enter a new name for the group:");
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();

                if (name != null)
                {
                    if (MWindow.Controller.Groups.ContainsKey(name))
                        await MWindow.ShowMessageAsync("Invalid Name", "There is already a group with that name.");
                    else
                    {
                        MWindow.Controller.Groups[name] = MWindow.Controller.Groups[groupName];
                        //Removing old
                        MWindow.Controller.Groups.Remove(groupName);
                        MWindow.Controller.AvailableApps.Remove(Constants.GroupHeader + groupName);

                        //Adding new
                        GroupCombo.ItemsSource = MWindow.Controller.Groups.Keys.ToList();
                        GroupCombo.SelectedItem = name;
                        MWindow.Controller.AvailableApps[Constants.GroupHeader + name] = Constants.GroupHeader + name;

                        int pos = Array.FindIndex(MWindow.Controller.GetState()[Constants.StateKeys], row => row == Constants.GroupHeader + groupName);
                        if (pos > 0)
                        {
                            MWindow.Controller.GetState()[Constants.StateKeys][pos] = Constants.GroupHeader + name;
                            MWindow.Controller.GetState()[Constants.StateNames][pos] = Constants.GroupHeader + name;
                            MWindow.UpdateMainViewCombos();
                        }
                        SaveState();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error on RenameGroup(): " + ex.ToString());
            }
        }

        //Função para deletar um grupo
        private async void DeleteGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string groupName = Convert.ToString(GroupCombo.SelectedItem) ?? throw new Exception();
                var name = await MWindow.ShowMessageAsync("Delete Group", "Are you sure you want to delete \"" + groupName + "\"?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative);


                if (name.ToString() == "Affirmative" && MWindow.Controller.Groups.ContainsKey(groupName))
                {
                    MWindow.Controller.Groups.Remove(groupName);
                    MWindow.Controller.AvailableApps.Remove(Constants.GroupHeader + groupName);
                    Selected_ListView.ItemsSource = null;
                    GroupCombo.ItemsSource = MWindow.Controller.Groups.Keys.ToList();
                    UpdateGroupSessions(groupName, true);
                    RenameButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    SaveState();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error on DeleteGroup(): " + ex.ToString());
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
                    MWindow.Controller.Groups[groupName].Add(selectedApp);
                    Selected_ListView.ItemsSource = MWindow.Controller.Groups[groupName].ToList();
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
                    MWindow.Controller.Groups[groupName].Remove(selectedApp);
                    Selected_ListView.ItemsSource = MWindow.Controller.Groups[groupName].ToList();
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
