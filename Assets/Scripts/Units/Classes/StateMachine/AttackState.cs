using Units.Enums;
using Units.Interfaces;
using Units.Structures;

namespace Units.Classes.StateMachine
{
    public class AttackState : UnitControllerState
    {
        private IUnitController _target;
        private float _swingTimer;
        private Attack _attack;
        
        public AttackState(IUnitController attacker, IUnitController target)
        {
            executor = attacker;
            _target = target;
            state = UnitState.Attack;
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
            executor.GetWorldView().PlayAttackPrep(1 / _swingTimer);
            _attack = executor.GetModel().GetAttack();
            _target.NotifyOfIncomingAttack(_attack);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            if (_swingTimer > 0) return;
            _target.GetDamage(_attack);
            executor.GetWorldView().PlayAttack();
            Finish();
        }
    }
}