using Units.Enums;
using Units.Interfaces;
using Units.Structures;

namespace Units.Classes.StateMachine
{
    public class StaggeringState : UnitControllerState
    {
        private float _staggeringTimer;
        private AttackOutcome _attackResult;
        
        public StaggeringState(IUnitController executor, AttackOutcome attackResult)
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
            _staggeringTimer = -_attackResult.HpChange * .5f;
            executor.GetWorldView().PlayTakeDamage(_attackResult);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _staggeringTimer -= dt;
            if (_staggeringTimer > 0) return;
            Finish();
        }
    }
}