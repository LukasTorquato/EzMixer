using AudioSwitcher.AudioApi.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolumeMixer.Views;

namespace VolumeMixer
{
    public class DeviceWatcher : IObserver<AudioSwitcher.AudioApi.DeviceChangedArgs>
    {
        public MainView view;

        public DeviceWatcher(MainView window)
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

        public void OnNext(AudioSwitcher.AudioApi.DeviceChangedArgs value)
        {
            view.Controller.UpdateCurrentDevice();
            view.Controller.SetSessionObserver(view.SCWatcher);
            view.Controller.SetSessionDObserver(view.SDWatcher);
            view.LoadState();
        }
    }
}
