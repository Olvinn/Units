using Units.Structures;

namespace Units.Interfaces
{
    public interface IUnitView
    {
        void PlayTakeDamage(Attack damage, AttackOutcome outcome);
    }
}