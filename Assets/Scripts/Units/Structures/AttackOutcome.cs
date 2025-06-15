using Units.Enums;

namespace Units.Structures
{
    public struct AttackOutcome
    {
        public AttackResult Result;
        public float HpChange;
        public LimbType? TargetLimb;
    }
}
