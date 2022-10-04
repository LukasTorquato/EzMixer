using System.Diagnostics;
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
        public LightingView()
        {
            InitializeComponent();
            //Volume1_RGB.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Volume_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

            
            if (e.Source == Volume1_ColorPicker && Volume1_ColorPicker.SelectedColor != null)
            {
                //Debug.WriteLine("Color 1: " + Volume1_ColorPicker.SelectedColor);
                var converter = new System.Windows.Media.BrushConverter();
                Volume1Ring.Stroke = (Brush)converter.ConvertFromString(Volume1_ColorPicker.SelectedColor.ToString());//new Color(255, 255, 255, 255);#FF836262
                
            } 
            else if (e.Source == Volume2_ColorPicker && Volume2_ColorPicker.SelectedColor != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                Volume2Ring.Stroke = (Brush)converter.ConvertFromString(Volume2_ColorPicker.SelectedColor.ToString());
               
            }
            else if (e.Source == Volume3_ColorPicker && Volume3_ColorPicker.SelectedColor != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                Volume3Ring.Stroke = (Brush)converter.ConvertFromString(Volume3_ColorPicker.SelectedColor.ToString());

            }
            else if (e.Source == Volume4_ColorPicker && Volume4_ColorPicker.SelectedColor != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                Volume4Ring.Stroke = (Brush)converter.ConvertFromString(Volume4_ColorPicker.SelectedColor.ToString());

            }
            else if (e.Source == Volume5_ColorPicker && Volume5_ColorPicker.SelectedColor != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                Volume5Ring.Stroke = (Brush)converter.ConvertFromString(Volume5_ColorPicker.SelectedColor.ToString());

            }

        }
    }
}
