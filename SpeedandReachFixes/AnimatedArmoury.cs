namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public class AnimatedArmoury
    {
        public class Keyword
        {
            private static readonly ModKey AnimatedArmoury = ModKey.FromNameAndExtension("NewArmoury.esp");

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeCestus = AnimatedArmoury.MakeFormKey(0x19aab3);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeClaw = AnimatedArmoury.MakeFormKey(0x19aab4);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeHalberd = AnimatedArmoury.MakeFormKey(0x0e4580);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypePike = AnimatedArmoury.MakeFormKey(0x0e457e);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeQtrStaff = AnimatedArmoury.MakeFormKey(0x0e4581);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeRapier = AnimatedArmoury.MakeFormKey(0x000801);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeSpear = AnimatedArmoury.MakeFormKey(0x0e457f);

            public static readonly FormLink<IKeywordCommonGetter> WeapTypeWhip = AnimatedArmoury.MakeFormKey(0x20f2a1);
        }
    }
}
