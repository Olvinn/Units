namespace Units.Health
{
    public interface IDamageable
    {
        public AttackOutcome TryBlockDamage(AttackData attackData);
        public AttackOutcome TryEvadeDamage(AttackData attackData);
        public AttackOutcome GetDamage(AttackData attackData);
    }
}
