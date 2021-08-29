using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE
{
    public static class NewArmoury
    {
        public static class Keyword
        {
            private static readonly ModKey ModKey = ModKey.FromNameAndExtension("NewArmoury.esp");
            public static FormKey NA_UniFWhp => ModKey.MakeFormKey(0x292ea1);
            public static FormKey WeapTypeRapier => ModKey.MakeFormKey(0x801);
            public static FormKey WeapTypePike => ModKey.MakeFormKey(0xe457e);
            public static FormKey WeapTypeSpear => ModKey.MakeFormKey(0xe457f);
            public static FormKey WeapTypeHalberd => ModKey.MakeFormKey(0xe4580);
            public static FormKey WeapTypeQtrStaff => ModKey.MakeFormKey(0xe4581);
            public static FormKey WeapTypeCestus => ModKey.MakeFormKey(0x19aab3);
            public static FormKey WeapTypeClaw => ModKey.MakeFormKey(0x19aab4);
            public static FormKey NA_CantParry => ModKey.MakeFormKey(0x19aab5);
            public static FormKey NA_InflictBleed => ModKey.MakeFormKey(0x1aef07);
            public static FormKey NAR_Sig => ModKey.MakeFormKey(0x20509e);
            public static FormKey WeapTypeWhip => ModKey.MakeFormKey(0x20f2a1);
            public static FormKey NA_UniIWhp => ModKey.MakeFormKey(0x292e9f);
            public static FormKey NA_UniEWhp => ModKey.MakeFormKey(0x292ea0);
            public static FormKey AltDagger => ModKey.MakeFormKey(0x2b65bb);
            public static FormKey AltMace => ModKey.MakeFormKey(0x2b65bc);
        }
    }
}
