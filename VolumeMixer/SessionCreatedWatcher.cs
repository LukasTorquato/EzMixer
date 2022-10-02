using AudioSwitcher.AudioApi.Session;
using VolumeMixer.Views;
using System;
using System.Diagnostics;
using System.Linq;

namespace VolumeMixer
{
    public class SessionCreatedWatcher : IObserver<IAudioSession>
    {
        public MainView view;

        public SessionCreatedWatcher(MainView window)
        {
            view = window;
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

            if (!view.Controller.AvailableApps.ContainsKey(pname))
            {
                if(value.DisplayName == "")
                    view.Controller.AvailableApps[pname] = pname;
                else
                    view.Controller.AvailableApps[pname] = value.DisplayName;

                view.ComboRefresh();
            }
            else
            {
                string[] selectedAppKeys = view.Controller.GetState()[Constants.StateKeys];
                for (int i = 0; i < selectedAppKeys.Length; i++)
                {
                    if (selectedAppKeys[i] == pname)
                        view.Controller.UpdateAudioSession(i, pname);
                }
            }
            
        }
    }
}
