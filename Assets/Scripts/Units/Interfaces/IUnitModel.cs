using Units.Classes;
using Units.Structures;

namespace Units.Interfaces
{
    public interface IUnitModel : IDamageable
    {
        string name { get; }
        UnitStats GetStats();
        bool CanAttack();
        Attack GetAttack();
        float GetSwingTime();
    }
}
