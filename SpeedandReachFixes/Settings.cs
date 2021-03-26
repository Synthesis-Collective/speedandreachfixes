using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedandReachFixes
{
    public class Settings
    {
        [SynthesisTooltip("This is an experimental feature.  It may cause issues.")]
        [SynthesisDescription("This is an experimental feature.  It may cause issues.")]
        [SynthesisOrder]
        public bool WeaponSwingAngleChanges = true;
    }
}
