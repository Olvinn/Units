using Units.Health;

namespace Units.Views
{
    public interface IUnitView
    {
        void PlayTakeDamage(AttackOutcome outcome);
    }
}