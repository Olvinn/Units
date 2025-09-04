using Units.Behaviour;
using Units.Health;
using Units.Stats;

namespace Units.Interfaces
{
    public interface IUnitModel : IDamageable
    {
        string name { get; }
        float GetFullHPPercent();
        UnitStats GetStats();
        bool CanAttack();
        AttackData GetAttack();
        float GetSwingTime();
        float GetTimeToBlock();
        float GetStaggerTime(AttackOutcome attack);
        UnitStateContainer GetStateContainer();
    }
}
