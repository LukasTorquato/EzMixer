using AudioSwitcher.AudioApi.Session;
using EzMixer.Views;
using System;
using System.Diagnostics;
using System.Linq;

namespace EzMixer
{
    public class SessionCreatedWatcher : IObserver<IAudioSession>
    {
        public MainWindow MWindow;

        public SessionCreatedWatcher(MainWindow window)
        {
            MWindow = window;
        }
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }


        public void OnNext(IAudioSession value)
        {
            string pname = Process.GetProcessById(value.ProcessId).ProcessName;

            // New app added to windows mixer
            if (!MWindow.Controller.AvailableApps.ContainsKey(pname))
            {
                if (value.DisplayName == "")
                    MWindow.Controller.AvailableApps[pname] = pname;
                else
                    MWindow.Controller.AvailableApps[pname] = value.DisplayName;

                MWindow.MView.ComboRefresh();
                MWindow.GView.UpdateListView();
            }
            //New app added to windows mixer and is previously selected by any combobox
            MWindow.Controller.ReloadAudioSessions();
            /*string[] selectedAppKeys = MWindow.Controller.GetState()[Constants.StateKeys];
            for (int i = 0; i < selectedAppKeys.Length; i++)
            {
                if (selectedAppKeys[i] == pname)
                    MWindow.Controller.UpdateAudioSession(i, pname);
                else if (selectedAppKeys[i].Contains(Constants.GroupHeader)) // check if is a group
                {
                    string groupName = selectedAppKeys[i].Replace(Constants.GroupHeader, "");
                    if(MWindow.Controller.Groups[groupName].Contains(value.DisplayName) || MWindow.Controller.Groups[groupName].Contains(pname)) // check if group contains session
                    {
                        MWindow.Controller.UpdateAudioSession(i, selectedAppKeys[i]);
                    }
                }
            }*/
            
        }
    }
}
