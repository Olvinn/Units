using Units.Health;

namespace Units.Controllers.ControllerStates
{
    public class StaggeringState : UnitControllerState
    {
        private float _staggeringTimer;
        private AttackOutcome _attackResult;
        
        public StaggeringState(IUnitStateMachine executor, AttackOutcome attackResult)
        {
            base.executor = executor;
            _attackResult = attackResult;
            stateEnum = UnitStateEnum.Staggering;
        }

        public override void Do()
        {
            base.Do();
            if (executor == null)
            {
                Finish();
                return;
            }
            _staggeringTimer = executor.GetModel().GetStaggerTime(_attackResult);
            executor.GetWorldView().PlayTakeDamage(_attackResult);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _staggeringTimer -= dt;
            if (_staggeringTimer > 0) return;
            Finish();
        }

        public override AttackOutcome TakeDamage(AttackData attack)
        {
            var result = executor.GetModel().GetDamage(attack);
            executor.GetWorldView().PlayTakeDamage(result);
            return result;
        }
    }
}