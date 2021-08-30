using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public static class WayOfTheMonk {
        public static class Keyword {
            private static readonly ModKey WayOfTheMonk = ModKey.FromNameAndExtension("WayOfTheMonk.esp");
            public static readonly FormLink<IKeywordGetter> WeapTypeUnarmed = WayOfTheMonk.MakeFormKey(0x05000D6D);
        }
    }
}
