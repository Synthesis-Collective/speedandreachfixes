using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
	/**
	 * @class WayOfTheMonk
	 * @brief Adds support for keywords from the Way Of The Monk mod.
	 * Currently maintained nexus page as of 21-09-03: https://www.nexusmods.com/skyrimspecialedition/mods/17684
	 */
	public static class WayOfTheMonk {
        public static class Keyword {
            private static readonly ModKey WayOfTheMonk = ModKey.FromNameAndExtension("WayOfTheMonk.esp");
            public static readonly FormLink<IKeywordGetter> WeapTypeUnarmed = WayOfTheMonk.MakeFormKey(0x05000D6D);
        }
    }
}
