using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VolumeMixer.Views
{
    /// <summary>
    /// Interaction logic for LightingView.xaml
    /// </summary>
    public partial class LightingView : UserControl
    {
        private readonly string filename = "../../../../Config/config.json";

        private string[] StagedLighting;

        private int numSliders;

        private Mixer Controller { get; set; }

        private SerialDevice Hardware;

        public LightingView(Mixer c, SerialDevice h, int n)
        {
            InitializeComponent();
            Controller = c;
            Hardware = h;
            numSliders = n;
            StagedLighting = new string[numSliders];

            Controller.GetState()[Constants.StateLighting].CopyTo(StagedLighting, 0);

            CheckBoxLoad();
            //Thread.Sleep(2000);
            ComboLoad();

        }

        private void CheckBoxLoad()
        {
            this.Dispatcher.Invoke(() =>
            {
                if(StagedLighting[0].Contains("*"))
                    Volume1_Checkbox.IsChecked = true;
                if(StagedLighting[1].Contains("*"))
                    Volume2_Checkbox.IsChecked = true;
                if (StagedLighting[2].Contains("*"))
                    Volume3_Checkbox.IsChecked = true;
                if (StagedLighting[3].Contains("*"))
                    Volume4_Checkbox.IsChecked = true;
                if (StagedLighting[4].Contains("*"))
                    Volume5_Checkbox.IsChecked = true;
            });
        }

        private void ComboLoad()
        {
            this.Dispatcher.Invoke(() =>
            {
                Volume1_ColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + StagedLighting[0].Replace("*", "").Replace("-", ""));
                Volume2_ColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + StagedLighting[1].Replace("*", "").Replace("-", ""));
                Volume3_ColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + StagedLighting[2].Replace("*", "").Replace("-", ""));
                Volume4_ColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + StagedLighting[3].Replace("*", "").Replace("-", ""));
                Volume5_ColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + StagedLighting[4].Replace("*", "").Replace("-", ""));
            });

            /*string message = "a5";
            for (int i = 0; i < numSliders; i++)
            {
                message += i + "=" + StagedLighting[i] + "|";
            }
            Hardware.LightingCommand = message;
            Hardware.UpdateLighting();*/

        }

        private void SaveState()
        {
            Dictionary<string, string[]> jsonDictionary = Controller.GetState();
            string jsonString = JsonSerializer.Serialize(jsonDictionary);
            File.WriteAllText(filename, jsonString);
        }

        private void Volume_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

            if (e.Source == Volume1_ColorPicker && Volume1_ColorPicker.SelectedColor != null)
            {
                //Debug.WriteLine("Color 1: " + Volume1_ColorPicker.SelectedColor);
                string colorstring = Volume1_ColorPicker.SelectedColor.ToString().Replace("#FF", ""); 
                //Color color = (Color)ColorConverter.ConvertFromString(colorstring); 

                var converter = new System.Windows.Media.BrushConverter();
                Volume1Ring.Stroke = (Brush)converter.ConvertFromString("#"+colorstring);

                StagedLighting[0] = colorstring;
                if (Volume1_Checkbox.IsChecked == true)
                    StagedLighting[0] += "*";
                else
                    StagedLighting[0] += "-";

                //Debug.WriteLine("Color RGB: " + StagedLighting[0]);

            } 
            else if (e.Source == Volume2_ColorPicker && Volume2_ColorPicker.SelectedColor != null)
            {
                string colorstring = Volume2_ColorPicker.SelectedColor.ToString().Replace("#FF", "");

                var converter = new System.Windows.Media.BrushConverter();
                Volume2Ring.Stroke = (Brush)converter.ConvertFromString("#" + colorstring);

                StagedLighting[1] = colorstring;
                if (Volume2_Checkbox.IsChecked == true)
                    StagedLighting[1] += "*";
                else
                    StagedLighting[1] += "-";

            }
            else if (e.Source == Volume3_ColorPicker && Volume3_ColorPicker.SelectedColor != null)
            {
                string colorstring = Volume3_ColorPicker.SelectedColor.ToString().Replace("#FF", "");

                var converter = new System.Windows.Media.BrushConverter();
                Volume3Ring.Stroke = (Brush)converter.ConvertFromString("#" + colorstring);

                StagedLighting[2] = colorstring;
                if (Volume3_Checkbox.IsChecked == true)
                    StagedLighting[2] += "*";
                else
                    StagedLighting[2] += "-";

            }
            else if (e.Source == Volume4_ColorPicker && Volume4_ColorPicker.SelectedColor != null)
            {
                string colorstring = Volume4_ColorPicker.SelectedColor.ToString().Replace("#FF", "");

                var converter = new System.Windows.Media.BrushConverter();
                Volume4Ring.Stroke = (Brush)converter.ConvertFromString("#" + colorstring);

                StagedLighting[3] = colorstring;
                if (Volume4_Checkbox.IsChecked == true)
                    StagedLighting[3] += "*";
                else
                    StagedLighting[3] += "-";

            }
            else if (e.Source == Volume5_ColorPicker && Volume5_ColorPicker.SelectedColor != null)
            {
                string colorstring = Volume5_ColorPicker.SelectedColor.ToString().Replace("#FF", "");

                var converter = new System.Windows.Media.BrushConverter();
                Volume5Ring.Stroke = (Brush)converter.ConvertFromString("#" + colorstring);

                StagedLighting[4] = colorstring;
                if (Volume5_Checkbox.IsChecked == true)
                    StagedLighting[4] += "*";
                else
                    StagedLighting[4] += "-";

            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            if (e.Source == Volume1_Checkbox)
            {
                if (Volume1_Checkbox.IsChecked == true)
                    StagedLighting[0] = StagedLighting[0].Replace("-", "*");
                else
                    StagedLighting[0] = StagedLighting[0].Replace("*", "-");
            } 
            else if (e.Source == Volume2_Checkbox)
            {
                if (Volume2_Checkbox.IsChecked == true)
                    StagedLighting[1] = StagedLighting[1].Replace("-", "*");
                else
                    StagedLighting[1] = StagedLighting[1].Replace("*", "-");
            }
            else if (e.Source == Volume3_Checkbox)
            {
                if (Volume3_Checkbox.IsChecked == true)
                    StagedLighting[2] = StagedLighting[2].Replace("-", "*");
                else
                    StagedLighting[2] = StagedLighting[2].Replace("*", "-");
            }
            else if (e.Source == Volume4_Checkbox)
            {
                if (Volume4_Checkbox.IsChecked == true)
                    StagedLighting[3] = StagedLighting[3].Replace("-", "*");
                else
                    StagedLighting[3] = StagedLighting[3].Replace("*", "-");
            }
            else if (e.Source == Volume5_Checkbox)
            {
                if (Volume5_Checkbox.IsChecked == true)
                    StagedLighting[4] = StagedLighting[4].Replace("-", "*");
                else
                    StagedLighting[4] = StagedLighting[4].Replace("*", "-");
            }
        }

        private void Apply_Lighting(object sender, RoutedEventArgs e)
        {
            ApplyLighting_Button.IsEnabled = false;

            string[] lightingstate = Controller.GetState()[Constants.StateLighting];
            string auxmessage = "";
            int ndiff = 0;

            for (int i = 0; i < numSliders; i++)
            {
                if (lightingstate[i] != StagedLighting[i])
                {
                    Controller.UpdateLighting(i, StagedLighting[i]);
                    auxmessage += i + "=" + StagedLighting[i] + "|";
                    ndiff++;

                }
            }

            if (ndiff > 0)
            {
                string message = "a" + ndiff + auxmessage;
                Hardware.LightingCommand = message;
                Hardware.UpdateLighting();
                SaveState();
            }
            ApplyLighting_Button.IsEnabled = true;

        }
    }
}
