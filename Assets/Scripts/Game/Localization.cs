using System.Collections.Generic;
using Units.Health;

namespace Game
{
    public static class Localization
    {
        public const string UILimbsPanelName = "Body State";
        public const string UIAttributesPanelName = "Attributes";
        public const string UIStatusPanelName = "Status";
        public const string UIStatsPanelName = "Stats";

        public static readonly Dictionary<LimbType, string> LimbsNames = new ()
        {
            { LimbType.Chest, "Chest" },
            { LimbType.Groin, "Groin" },
            { LimbType.Head, "Head" },
            { LimbType.Neck, "Neck" },
            { LimbType.LeftArm, "Left arm" },
            { LimbType.RightArm, "Right arm" },
            { LimbType.Stomach, "Stomach" },
            { LimbType.LeftLeg, "Left Leg" },
            { LimbType.RightLeg, "Right Leg" },
        };
    }
}