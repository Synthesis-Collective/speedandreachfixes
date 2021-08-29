using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public class NewArmoury
    {
        public class Keyword
        {
            private static readonly ModKey AnimatedArmoury = ModKey.FromNameAndExtension("NewArmoury.esp");
            public static readonly FormLink<IKeywordGetter> WeapTypeCestus = AnimatedArmoury.MakeFormKey(0x19aab3);
            public static readonly FormLink<IKeywordGetter> WeapTypeClaw = AnimatedArmoury.MakeFormKey(0x19aab4);
            public static readonly FormLink<IKeywordGetter> WeapTypeHalberd = AnimatedArmoury.MakeFormKey(0x0e4580);
            public static readonly FormLink<IKeywordGetter> WeapTypePike = AnimatedArmoury.MakeFormKey(0x0e457e);
            public static readonly FormLink<IKeywordGetter> WeapTypeQtrStaff = AnimatedArmoury.MakeFormKey(0x0e4581);
            public static readonly FormLink<IKeywordGetter> WeapTypeRapier = AnimatedArmoury.MakeFormKey(0x000801);
            public static readonly FormLink<IKeywordGetter> WeapTypeSpear = AnimatedArmoury.MakeFormKey(0x0e457f);
            public static readonly FormLink<IKeywordGetter> WeapTypeWhip = AnimatedArmoury.MakeFormKey(0x20f2a1);
        }
    }
}