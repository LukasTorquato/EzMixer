﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzMixer
{
    public class Program
    {
        [System.STAThreadAttribute()]
        public static void Main()
        {
            var mainApp = new EzMixer.App();
            mainApp.InitializeComponent();
            mainApp.Run();
        }
    }
}
