using Units.Structures;

namespace Units.Interfaces
{
    public interface IDamageable
    {
        public AttackOutcome TakeDamage(Attack attack);
    }
}
