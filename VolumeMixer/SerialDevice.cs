using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VolumeMixer
{
    public class SerialDevice
    {
        public System.IO.Ports.SerialPort device;

        public bool Connected { get; set; }

        public string LightingCommand { get; set; }

        private readonly int numSliders;
        //Regex para leitura do mixer de 4 e 5 volumes
        //private readonly string pattern = @"^(\d{1,4}[|]\d{1,4}[|\s]\d{1,4}[|\s]\d{1,4})([|\s]\d{1,4})?$";

        public void ScanDevicePort()
        {
            foreach (string port in System.IO.Ports.SerialPort.GetPortNames())
            {
                device.PortName = port;

                try
                {
                    device.Open();
                    if (device.IsOpen)
                    {
                        ClearInBuffer();
                        Thread.Sleep(10);
                        string testread = device.ReadLine().Replace("\n", "").Replace("\r", "");
                        Debug.WriteLine("Test Read: " + testread);
                        if (!(testread is null))
                        {
                            bool portfound = Regex.IsMatch(testread, Constants.RegPattern);
                            Debug.WriteLine("Regex: " + portfound);
                            if (portfound)
                            {
                                Connected = true;
                                return;
                            }
                            else
                                throw new IOException(); 
                        }
                    }
                }
                catch //(Exception ex)System.TimeoutException
                {
                    device.Close();
                    Connected = false;
                    //Debug.WriteLine("Invalid Port: " + port + " Error: " + ex.Message);
                }
            }
        }

        public double[] GetVolume(int sensibility)
        {
            double[] volumes = new double[numSliders];

            try
            {
                string[] strvol = device.ReadLine().Replace("\n", "").Replace("\r", "").Split("|");
                for (int i = 0; i < numSliders; i++)
                {
                    volumes[i] = (Math.Floor(double.Parse(strvol[i]) / (sensibility*10)))*sensibility;
                }
            }
            catch (System.InvalidOperationException ex)
            {
                Debug.WriteLine("Error on GetVolume(): Ezmixer Disconnected. - " + ex.Message);
                Connected = false;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Error on GetVolume(): " + ex.InnerException + ex.Message);
            }
            return volumes;
        }

        public void ClearInBuffer() => device.DiscardInBuffer();
        
        public void UpdateLighting()
        {
            //color = 255,0,255* -> a0-255,0,255*
            try
            {
                device.DiscardOutBuffer();
                Debug.WriteLine("Message: "+ LightingCommand);
                device.WriteLine(LightingCommand);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on UpdateLighting(): " + ex.InnerException + ex.Message);
            }
        }

        public SerialDevice(int _numsliders)
        {
            Connected = false;
            LightingCommand = Constants.StockLighting;
            device = new System.IO.Ports.SerialPort
            {
                BaudRate = 115200,
                ReadTimeout = 2000
            };
            numSliders = _numsliders;
            ScanDevicePort();

        }

    }
}
