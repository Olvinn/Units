using Units.Structures;

namespace Units.Interfaces
{
    public interface IDamageable
    {
        public AttackOutcome TryBlockDamage(Attack attack);
        public AttackOutcome TryEvadeDamage(Attack attack);
        public AttackOutcome GetDamage(Attack attack);
    }
}
