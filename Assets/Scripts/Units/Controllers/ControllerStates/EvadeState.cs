using Units.Health;
using Units.Views;
using UnityEngine;

namespace Units.Controllers.ControllerStates
{
    public class EvadeState : UnitControllerState
    {
        private float _swingTimer;
        private AttackData _attackData;
        
        public EvadeState(IUnitStateMachine executor, AttackData attackData)
        {
            base.executor = executor;
            _attackData = attackData;
            stateEnum = UnitStateEnum.Evading;
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
            executor.GetMovement().Stop();
            executor.GetWorldView().Play(Cue.Evade);
            Finish();
        }
        
        public override AttackOutcome TakeDamage(AttackData attack)
        {
            var result = executor.GetModel().TryEvadeDamage(attack);
            if (result.ResultType != AttackResultType.Evaded)
                executor.GetWorldView().PlayTakeDamage(result);
            else
                executor.GetWorldView().Play(Cue.Block);
            return result;
        }
    }
}