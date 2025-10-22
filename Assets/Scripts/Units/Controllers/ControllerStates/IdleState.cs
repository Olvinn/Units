using Units.Health;
using Units.Views;

namespace Units.Controllers.ControllerStates
{
    public class IdleState : UnitControllerState
    {
        public IdleState()
        {
            stateEnum = UnitStateEnum.Idle;
        }

        public override bool CanAttack() => true;
        public override bool CanEvade() => true;
        public override bool CanMove() => true;
        public override bool CanBlock() => true;

        public override AttackOutcome TakeDamage(AttackData attack)
        {
            var result = executor.GetModel().GetDamage(attack);
            executor.GetWorldView().PlayTakeDamage(result);
            return result;
        }
    }
}