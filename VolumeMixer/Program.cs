using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeMixer
{
    public class Program
    {
        [System.STAThreadAttribute()]
        public static void Main()
        {
            var mainApp = new VolumeMixer.App();
            mainApp.InitializeComponent();
            mainApp.Run();
        }
    }
}
