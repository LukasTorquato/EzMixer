using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EzMixer
{
    public class Mixer
    {
        // Controlador geral da biblioteca do mixer
        private CoreAudioController audioController;
        // Lista de devices disponível na máquina
        private List<CoreAudioDevice> PlaybackDevices { get; set; }
        //private List<CoreAudioDevice> RecordDevices { get; set; }
        // Dispositivo de reprodução atual
        private CoreAudioDevice CurrentPlaybackDevice;
        // Dispositivo de Microfone atual
        private CoreAudioDevice CurrentRecordDevice;
        // Estado atual do Mixer - Apps escolhidos e a iluminação de cada controle
        private Dictionary<string, string[]> state;
        // Lista guarda as sessions do mixer utilizada em cada controle
        private List<IAudioSession>[] audioSessions;
        // Posição assinalada para o controle que comanda o master volume
        private int masterAssignedPosition;
        // Posição assinalada para o controle que comanda o microfone
        private int micAssignedPosition;
        // Número de controles que o mixer possui
        private int numSliders { get; set; }
        // Dic para guardar os APPs disponíveis no Mixer
        public Dictionary<string, string> AvailableApps { get; set; }
        // Dic para guardar os grupos
        public Dictionary<string, List<string>> Groups { get; set; }



        //função para atualizar os dispositivos de playback e record atuais
        public void UpdateCurrentDevice()
        {
            //CoreAudioController controller = new CoreAudioController();
            CurrentPlaybackDevice = audioController.DefaultPlaybackDevice;
            CurrentRecordDevice = audioController.DefaultCaptureDevice;
        }

        public void UpdatePlaybackDevices()
        {
            //CoreAudioController controller = new CoreAudioController();
            PlaybackDevices.Clear();
            foreach (var device in audioController.GetDevices())
            {
                if (device.State.ToString() == "Active" && device.IsPlaybackDevice == true)
                    PlaybackDevices.Add(device);
            }
        }

        //função para atualizar a session para um determinado controle
        public void UpdateAudioSession(int pos, string name, bool clearopt=false)
        {
            audioSessions[pos].Clear();

            if(clearopt)
                return;

            if (name == Constants.Master)
            {
                //Deixa a posição "pos" nula no array de audioSessions
                masterAssignedPosition = pos;
                state[Constants.StateKeys][pos] = state[Constants.StateNames][pos] = Constants.Master;
            }
            else if (name == Constants.Mic)
            {
                micAssignedPosition = pos;
                state[Constants.StateKeys][pos] = state[Constants.StateNames][pos] = Constants.Mic;
            }
            else
            {
                if (masterAssignedPosition == pos)
                    masterAssignedPosition = -1;
                else if (micAssignedPosition == pos)
                    micAssignedPosition = -1;

                string pname;
                foreach (var device in PlaybackDevices)
                {
                    foreach (var session in device.SessionController)
                    {
                        pname = Process.GetProcessById(session.ProcessId).ProcessName;
                        //Debug.WriteLine(pname);
                        if (name.StartsWith(Constants.GroupHeader))
                        {
                            string groupName = name.Replace(Constants.GroupHeader, "");
                            /*if (!Groups.ContainsKey(groupName))
                            {
                                state[Constants.StateKeys][pos] = state[Constants.StateNames][pos] = null;
                                return;
                            }*/
                            if (Groups[groupName].Contains(pname) || Groups[groupName].Contains(session.DisplayName) ||
                               (Groups[groupName].Contains(Constants.SysSounds) && session.DisplayName.Contains("System32"))) //Caso System Sounds
                            {
                                audioSessions[pos].Add(session);
                            }
                            state[Constants.StateKeys][pos] = state[Constants.StateNames][pos] = Constants.GroupHeader+groupName;

                        }
                        else if (pname == name || session.DisplayName == name)
                        {
                            audioSessions[pos].Add(session);
                            state[Constants.StateKeys][pos] = pname;
                            state[Constants.StateNames][pos] = name;
                        }
                        else if (name == Constants.SysSounds && session.DisplayName.Contains("System32"))
                        {
                            audioSessions[pos].Add(session);
                            state[Constants.StateKeys][pos] = pname;
                            state[Constants.StateNames][pos] = name;
                        }
                        else if (AvailableApps.ContainsValue(name))
                        {
                            state[Constants.StateKeys][pos] = AvailableApps.FirstOrDefault(x => x.Value == name).Key;
                            state[Constants.StateNames][pos] = name;
                        }
                    }
                }
            }
        }

        //função para atualizar a iluminação para um determinado controle
        public void UpdateLighting(int pos, string color)
        {
            state[Constants.StateLighting][pos] = color;
        }

        //função que retorna o volume atual de um determinado controle
        public double GetSessionVolume(int pos)
        {
            if (masterAssignedPosition == pos)
                return CurrentPlaybackDevice.Volume;
            else if (micAssignedPosition == pos)
                return CurrentRecordDevice.Volume;
            else if(audioSessions[pos] != null)
                return audioSessions[pos].First().Volume;
            else
                return 0;
        }

        //função que seta o volume atual para um determinado controle
        public void SetSessionVolume(int pos, double vol)
        {
            if (masterAssignedPosition == pos)
                CurrentPlaybackDevice.Volume = vol;
            else if (micAssignedPosition == pos)
                CurrentRecordDevice.Volume = vol;
            else if (audioSessions[pos] != null)
            {
                foreach (var session in audioSessions[pos])
                    session.Volume = vol;
            }
                
        }

        //função que retona os Apps disponíveis no mixer do windows
        public void StartAvailableApps()
        {
            string pname;
            AvailableApps[Constants.Master] = Constants.Master;
            AvailableApps[Constants.Mic] = Constants.Mic;

            foreach (var session in CurrentPlaybackDevice.SessionController)
            {
                pname = Process.GetProcessById(session.ProcessId).ProcessName;
                if (session.DisplayName == "")
                    AvailableApps[pname] = pname;
                else
                {
                    if (session.DisplayName.Contains("System32"))
                        AvailableApps[pname] = Constants.SysSounds;
                    else
                        AvailableApps[pname] = session.DisplayName;
                }
            }
        }

        //função que carrega o estado do arquivo para o atual do mixer
        public void LoadState(Dictionary<string, string[]> jsonState)
        {
            state = jsonState;

            for (int i = 0; i < numSliders; i++)
            {
                if (state[Constants.StateKeys][i] != null)
                {
                    UpdateAudioSession(i, state[Constants.StateNames][i]);
                    if (AvailableApps.ContainsKey(state[Constants.StateKeys][i]) == false)
                        AvailableApps[state[Constants.StateKeys][i]] = state[Constants.StateNames][i];
                }
            }
            foreach(var group in Groups)
            {
                AvailableApps[Constants.GroupHeader + group.Key] = Constants.GroupHeader+group.Key;
            }
            
        }

        //função que retorna o estado atual do mixer
        public Dictionary<string, string[]> GetState() => state;
        public void SetSessionObserver(SessionCreatedWatcher watcher) => CurrentPlaybackDevice.SessionController.SessionCreated.Subscribe(watcher);
        public void SetSessionDObserver(SessionDisconnectedWatcher watcher) => CurrentPlaybackDevice.SessionController.SessionDisconnected.Subscribe(watcher);
        public static void SetDeviceObserver(DeviceWatcher watcher) => new CoreAudioController().AudioDeviceChanged.Subscribe(watcher);

        public Mixer(int _numSliders)
        {
            numSliders = _numSliders;
            masterAssignedPosition = -1;
            micAssignedPosition = -1;

            audioController = new CoreAudioController();

            CurrentPlaybackDevice = audioController.DefaultPlaybackDevice;
            CurrentRecordDevice = audioController.DefaultCaptureDevice;

            PlaybackDevices = new List<CoreAudioDevice>();
            //RecordDevices = new List<CoreAudioDevice>();

            foreach (var device in audioController.GetDevices())
            {
                if (device.State.ToString() == "Active" && device.IsPlaybackDevice == true)
                    PlaybackDevices.Add(device);
                //else if (device.State.ToString() == "Active" && device.IsCaptureDevice == true)
                //RecordDevices.Add(device);

            }

            audioSessions = new List<IAudioSession>[numSliders];
            for (int i = 0; i < numSliders; i++)
            {
                audioSessions[i] = new List<IAudioSession>();
            }
            Groups = new Dictionary<string, List<string>>();
            AvailableApps = new Dictionary<string, string>();
            StartAvailableApps();

            state = new Dictionary<string, string[]>();
            /*.{
                [Constants.StateKeys] = new string[numSliders],
                [Constants.StateNames] = new string[numSliders],
                [Constants.StateLighting] = new string[numSliders]
            };*/
        }
    }
}
