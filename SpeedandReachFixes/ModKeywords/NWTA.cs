using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
	/**
	 * @class NWTA
	 * @brief Adds support for keywords from the New Weapon Types and Animation Support mod.
	 * Currently maintained nexus page as of 21-09-03: https://www.nexusmods.com/skyrimspecialedition/mods/33985
	 */
	public static class NWTA
    {
        public static class Keyword
        {
            private static readonly ModKey NWTA = ModKey.FromNameAndExtension("NWTA.esp");
            public static readonly FormLink<IKeywordGetter> WeapTypeKatana = NWTA.MakeFormKey(0x000D61);
            public static readonly FormLink<IKeywordGetter> WeapTypeCurvedSword = NWTA.MakeFormKey(0x000D71);
        }
    }
}
