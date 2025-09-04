using Units.Health;

namespace Units.Controllers.ControllerStates
{
    public class AttackState : UnitControllerState
    {
        private IUnitController _target;
        private float _swingTimer, _finishTimer;
        private AttackData _attackData;
        private bool _attacked;
        
        public AttackState(IUnitController attacker, IUnitController target)
        {
            executor = attacker;
            _target = target;
            stateEnum = UnitStateEnum.Attack;
            _attacked = false;
        }

        public override void Do()
        {
            base.Do();
            if (_target == null || executor == null)
            {
                Finish();
                return;
            }
            _swingTimer = executor.GetModel().GetSwingTime();
            _finishTimer = _swingTimer * 2;
            executor.GetWorldView().PlayAttackPrep(1 / _swingTimer);
            _attackData = executor.GetModel().GetAttack();
            _target.NotifyOfIncomingAttack(_attackData);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            _finishTimer -= dt;
            if (_swingTimer > 0) return;
            if (!_attacked)
            {
                _target.TakeDamage(_attackData);
                executor.GetWorldView().PlayAttack();
                _attacked = true;
            }
            if (_finishTimer > 0) return;
            Finish(); 
        }
    }
}