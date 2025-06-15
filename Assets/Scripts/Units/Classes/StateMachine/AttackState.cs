using Units.Enums;
using Units.Interfaces;
using Units.Structures;

namespace Units.Classes.StateMachine
{
    public class AttackState : UnitControllerState
    {
        private IUnitController _target;
        private float _swingTimer, _finishTimer;
        private Attack _attack;
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
            _attack = executor.GetModel().GetAttack();
            _target.NotifyOfIncomingAttack(_attack);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            _finishTimer -= dt;
            if (_swingTimer > 0) return;
            if (!_attacked)
            {
                _target.GetDamage(_attack);
                executor.GetWorldView().PlayAttack();
                _attacked = true;
            }
            if (_finishTimer > 0) return;
            Finish(); 
        }
    }
}