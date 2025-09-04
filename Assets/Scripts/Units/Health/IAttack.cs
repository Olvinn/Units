namespace Units.Health
{
    public interface IAttack
    {
        public float attackMod { get; }
        public DamageData damageData { get; }
        public AttackType type { get; }
    }
}
