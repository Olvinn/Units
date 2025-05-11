using Units.Enums;
using Units.Structures;

namespace Units.Interfaces
{
    public interface IAttack
    {
        public float attackMod { get; }
        public Damage damage { get; }
        public AttackType type { get; }
    }
}
