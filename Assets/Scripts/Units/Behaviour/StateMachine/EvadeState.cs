using Units.Health;
using Units.Interfaces;
using UnityEngine;

namespace Units.Behaviour.StateMachine
{
    public class EvadeState : UnitControllerState
    {
        private float _swingTimer;
        private AttackData _attackData;
        
        public EvadeState(IUnitController executor, AttackData attackData)
        {
            base.executor = executor;
            base.executor.onTakeDamage += OnTakeDamage; 
            _attackData = attackData;
            stateEnum = UnitStateEnum.Evading;
        }

        private void OnTakeDamage(AttackOutcome result)
        {
            if (result.ResultType != AttackResultType.Evaded)
                executor.GetWorldView().PlayTakeDamage(result);
            Finish();
        }

        public override void Do()
        {
            base.Do();
            if (executor == null)
            {
                Finish();
                return;
            }
            _swingTimer = _attackData.ApproxHitTime - Time.time;
            _swingTimer *= 2;
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            if (_swingTimer > 0) return;
            executor.GetWorldView().PlayEvasion();
            Finish();
        }
    }
}