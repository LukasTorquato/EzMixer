using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeMixer
{
    static internal class Constants
    {
        public const string Master = "Master";

        public const string Mic = "Microphone";

        public const string SysSounds = "System Sounds";

        public const string StateKeys = "selectedAppKeys";

        public const string StateNames = "selectedAppNames";

        public const string StateLighting = "selectedLighting";

        public const string DiscordAlias = "Discord Voice";

        public const string FileLocation = "../../../../config.json";

        public const string StockLighting = "a50=FFFFFF*|1=FFFFFF*|2=FFFFFF*|3=FFFFFF*|4=FFFFFF*|";

        public const string RegPattern = @"^(\d{1,4}[|]\d{1,4}[|\s]\d{1,4}[|\s]\d{1,4})([|\s]\d{1,4})?$";

        public const string DiscReg = @"^Discord\d+$";

    }
}
