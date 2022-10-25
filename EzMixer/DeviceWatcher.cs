using AudioSwitcher.AudioApi.Session;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EzMixer.Views;

namespace EzMixer
{
    public class DeviceWatcher : IObserver<AudioSwitcher.AudioApi.DeviceChangedArgs>
    {
        public MainWindow view;

        public DeviceWatcher(MainWindow window)
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
            
            if (value.Device.IsDefaultDevice == true && value.ChangedType.ToString() == "DefaultChanged")
            {
                view.Controller.UpdateCurrentDevice();
                view.Controller.SetSessionObserver(view.SCWatcher);
                view.Controller.SetSessionDObserver(view.SDWatcher);
            }
            else if(value.ChangedType.ToString() == "StateChanged")
            {
                view.Controller.UpdatePlaybackDevices();
            }
        }
    }
}
