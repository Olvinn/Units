using Units.Controllers;
using Units.Health;
using Units.Stats;
using Units.Views;

namespace Units.Models
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
