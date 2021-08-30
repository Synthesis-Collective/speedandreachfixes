using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public static class NWTA // New Weapon Types and Animations
    {
        public static class Keyword
        {
            private static readonly ModKey NWTA = ModKey.FromNameAndExtension("NWTA.esp");
            public static readonly FormLink<IKeywordGetter> WeapTypeKatana = NWTA.MakeFormKey(0x000D61);
            public static readonly FormLink<IKeywordGetter> WeapTypeCurvedSword = NWTA.MakeFormKey(0x000D71);
        }
    }
}
