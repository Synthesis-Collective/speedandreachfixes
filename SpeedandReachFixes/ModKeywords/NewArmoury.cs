using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public static class NewArmoury
    {
        public static class Keyword
        {
            private static readonly ModKey NewArmoury = ModKey.FromNameAndExtension("NewArmoury.esp");
            public static readonly FormLink<IKeywordGetter> WeapTypeCestus = NewArmoury.MakeFormKey(0x19aab3);
            public static readonly FormLink<IKeywordGetter> WeapTypeClaw = NewArmoury.MakeFormKey(0x19aab4);
            public static readonly FormLink<IKeywordGetter> WeapTypeHalberd = NewArmoury.MakeFormKey(0x0e4580);
            public static readonly FormLink<IKeywordGetter> WeapTypePike = NewArmoury.MakeFormKey(0x0e457e);
            public static readonly FormLink<IKeywordGetter> WeapTypeQtrStaff = NewArmoury.MakeFormKey(0x0e4581);
            public static readonly FormLink<IKeywordGetter> WeapTypeRapier = NewArmoury.MakeFormKey(0x000801);
            public static readonly FormLink<IKeywordGetter> WeapTypeSpear = NewArmoury.MakeFormKey(0x0e457f);
            public static readonly FormLink<IKeywordGetter> WeapTypeWhip = NewArmoury.MakeFormKey(0x31BD77);
        }
    }
}