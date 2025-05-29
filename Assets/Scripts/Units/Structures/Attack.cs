using Units.Enums;
using Units.Interfaces;

namespace Units.Structures
{
    public struct Attack
    {
#nullable enable
        public IUnitController? Source;
#nullable disable
        public float AttackMod;
        public Damage[] Damage;
        public AttackType Type;
        public bool IsCritical, IsSneak;
        public float ApproxHitTime;

        public LimbType? LimbTarget;
        
#nullable enable
        public Attack(IUnitController? attacker, float attackMod, Damage[] damage, AttackType type, bool critical, bool isSneak,
            LimbType? limbTarget, float approxHitTime)
        {
            Source = attacker;
            AttackMod = attackMod;
            Damage = damage;
            Type = type;
            IsCritical = critical;
            IsSneak = isSneak;
            LimbTarget = limbTarget;
            ApproxHitTime = approxHitTime;
        }
    }
#nullable disable
}