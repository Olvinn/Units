using Units.Enums;
using Units.Interfaces;

namespace Units.Structures
{
    public struct Attack
    {
#nullable enable
        public IUnitModel? Source;
#nullable disable
        public float AttackMod;
        public Damage[] Damage;
        public AttackType Type;
        public bool IsCritical, IsSneak;

        public LimbType? LimbTarget;
        
#nullable enable
        public Attack(IUnitModel? attacker, float attackMod, Damage[] damage, AttackType type, bool critical, bool isSneak,
            LimbType? limbTarget)
        {
            Source = attacker;
            AttackMod = attackMod;
            Damage = damage;
            Type = type;
            IsCritical = critical;
            IsSneak = isSneak;
            LimbTarget = limbTarget;
        }
    }
#nullable disable
}