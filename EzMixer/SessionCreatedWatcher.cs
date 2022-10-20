using AudioSwitcher.AudioApi.Session;
using EzMixer.Views;
using System;
using System.Diagnostics;
using System.Linq;

namespace EzMixer
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

            // New app added to windows mixer
            if (!view.Controller.AvailableApps.ContainsKey(pname))
            {
                if (value.DisplayName == "")
                    view.Controller.AvailableApps[pname] = pname;
                else
                    view.Controller.AvailableApps[pname] = value.DisplayName;

                view.ComboRefresh();
            }
            //New app added to windows mixer and is previously selected by any combobox
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
