using Units.Health;
using UnityEngine;

namespace Units.Controllers.ControllerStates
{
    public class AttackState : UnitControllerState
    {
        private IUnitController _target;
        private float _swingTimer, _finishTimer;
        private AttackData _attackData;
        private bool _isAttackDone, _attackWasInitialized;
        
        public AttackState(IUnitController attacker, IUnitController target)
        {
            executor = attacker;
            _target = target;
            stateEnum = UnitStateEnum.Attack;
            _isAttackDone = false;
            base.executor.GetMovement().onReachDestination += OnReachTarget; 
        }

        public override void Do()
        {
            base.Do();
            if (_target == null || executor == null)
            {
                Finish();
                return;
            }
            
        }

        public override void Update(float dt)
        {
            if (Vector3.Distance(executor.GetPosition(), _target.GetPosition()) > .5f)
            {
                executor.GetMovement().Move(_target.GetPosition());
                _attackWasInitialized = false;
                return;
            }
            else if (!_attackWasInitialized)
            {
                executor.GetMovement().Stop();
                _swingTimer = executor.GetModel().GetSwingTime();
                _finishTimer = _swingTimer * 2;
                executor.GetWorldView().PlayAttackPrep(1 / _swingTimer);
                _attackData = executor.GetModel().GetAttack();
                _target.NotifyOfIncomingAttack(_attackData);
                _attackWasInitialized = true;
            }
            
            if (!isActive) return;
            _swingTimer -= dt;
            _finishTimer -= dt;
            if (_swingTimer > 0) return;
            if (!_isAttackDone)
            {
                _target.TakeDamage(_attackData);
                executor.GetWorldView().PlayAttack();
                _isAttackDone = true;
            }
            if (_finishTimer > 0) return;
            Finish(); 
        }

        private void OnReachTarget()
        {
            
        }

        public override void Finish()
        {
            base.Finish();
            base.executor.GetMovement().onReachDestination -= OnReachTarget; 
        }
    }
}