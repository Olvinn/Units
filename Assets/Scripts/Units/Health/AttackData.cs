using Units.Interfaces;

namespace Units.Health
{
    public struct AttackData
    {
#nullable enable
        public IUnitController? Source;
#nullable disable
        public float AttackMod;
        public DamageData[] Damage;
        public AttackType Type;
        public bool IsCritical, IsSneak;
        public float ApproxHitTime;

        public LimbType? TargetLimb;
        
#nullable enable
        public AttackData(IUnitController? attacker, float attackMod, DamageData[] damage, AttackType type, bool critical, bool isSneak,
            LimbType? targetLimb, float approxHitTime)
        {
            Source = attacker;
            AttackMod = attackMod;
            Damage = damage;
            Type = type;
            IsCritical = critical;
            IsSneak = isSneak;
            TargetLimb = targetLimb;
            ApproxHitTime = approxHitTime;
        }
    }
#nullable disable
}