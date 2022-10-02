using AudioSwitcher.AudioApi.Session;
using VolumeMixer.Views;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;

namespace VolumeMixer
{
    public class SessionDisconnectedWatcher : IObserver<string>
    {
        public MainView view;

        public SessionDisconnectedWatcher(MainView window)
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


        public void OnNext(string value)
        {
            string[] selectedAppKeys = view.Controller.GetState()[Constants.StateKeys];
            string KeyToRemove = "";

            foreach(string availableAppKey in view.Controller.AvailableApps.Keys.ToList())
            {
                if (value.Contains(availableAppKey) && !selectedAppKeys.Contains(availableAppKey))
                    KeyToRemove = availableAppKey;
            }
            if (KeyToRemove != "")
            {
                view.Controller.AvailableApps.Remove(KeyToRemove);
                view.ComboRefresh();
            }

        }
    }
}
