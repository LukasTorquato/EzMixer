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

                MWindow.MainView.ComboRefresh();
                MWindow.GroupView.UpdateListView();
            }
            //New app added to windows mixer and is previously selected by any combobox
            MWindow.Controller.ReloadAudioSessions();
            
        }
    }
}
