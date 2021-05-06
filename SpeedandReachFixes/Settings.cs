using Mutagen.Bethesda.Synthesis.Settings;

namespace SpeedandReachFixes
{
    public class Settings
    {
        [SynthesisTooltip("This is an experimental feature.  It may cause issues.")]
        [SynthesisDescription("This is an experimental feature.  It may cause issues.")]
        [SynthesisOrder]
        public bool WeaponSwingAngleChanges = true;

        [SynthesisOrder]
        public SpeedSettings Battleaxe = new()
        {
            Reach = 0.8275F,
            Speed = 0.666667F,
        };

        [SynthesisOrder]
        public SpeedSettings Dagger = new()
        {
            Reach = 0.533F,
            Speed = 1.35F,
        };

        [SynthesisOrder]
        public SpeedSettings Greatsword = new()
        {
            Reach = 0.88F,
            Speed = 0.85F,
        };

        [SynthesisOrder]
        public SpeedSettings Mace = new()
        {
            Reach = 0.75F,
            Speed = 0.9F,
        };

        [SynthesisOrder]
        public SpeedSettings Sword = new()
        {
            Reach = 0.83F,
            Speed = 1.1F,
        };

        [SynthesisOrder]
        public SpeedSettings WarAxe = new()
        {
            Reach = 0.6F,
            Speed = 1F,
        };

        [SynthesisOrder]
        public SpeedSettings Warhammer = new()
        {
            Reach = 0.8F,
            Speed = 0.6F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Cestus = new()
        {
            Reach = 0F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Claw = new()
        {
            Reach = 0.41F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Halberd = new()
        {
            Reach = 0.58F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Pike = new()
        {
            Reach = 0.2F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings QuarterStaff = new()
        {
            Reach = 0.25F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Rapier = new()
        {
            Reach = 0.2F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Spear = new()
        {
            Reach = 0F,
        };

        [SynthesisOrder]
        public ItemCategoryReachSettings Whip = new()
        {
            Reach = 0.5F,
        };
    }

    public class ItemCategoryReachSettings
    {
        public float Reach;
    }

    public class SpeedSettings : ItemCategoryReachSettings
    {
        public float Speed;
    }
}
