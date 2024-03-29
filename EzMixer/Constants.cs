﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzMixer
{
    static internal class Constants
    {
        public const string Master = "Master";

        public const string Mic = "Microphone";

        public const string SysSounds = "System Sounds";

        public const string StateKeys = "selectedAppKeys";

        public const string StateNames = "selectedAppNames";

        public const string StateLighting = "selectedLighting";

        public const string StockColor = "808080"; // Gray

        public const string ExitOnCloseKey = "EOC";

        public const string WindowsStartupKey = "WinStart";

        public const string StartMinimizedKey = "SMin";

        public const string PollingRateKey = "PollingRate";

        public const string SensibilityKey = "Sens";

        public const string FileLocation = "../../../config.json";

        public const string PrefFileLocation = "../../../preferences.json";

        public const string GroupsFileLocation = "../../../groups.json";

        public const string GroupHeader = "Group: ";

        public const string RegistryKey = @"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\";

        public const string StockLighting = "a50=FFFFFF*|1=FFFFFF*|2=FFFFFF*|3=FFFFFF*|4=FFFFFF*|";

        public const string RegPattern = @"^(\d{1,4}[-]\d{1,4}[-\s]\d{1,4}[-\s]\d{1,4})([-\s]\d{1,4})?$";

        public const string RegistryKeyPath = @"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

    }
}
