using Units.Health;
using UnityEngine;

namespace Units.Controllers.ControllerStates
{
    public class BlockState : UnitControllerState
    {
        private float _swingTimer, _endTimer;
        private AttackData _attackData;
        
        public BlockState(IUnitController executor, AttackData attackData)
        {
            base.executor = executor;
            base.executor.onTakeDamage += OnTakeDamage; 
            _attackData = attackData;
            stateEnum = UnitStateEnum.BlockPrep;
        }

        private void OnTakeDamage(AttackOutcome result)
        {
            if (result.ResultType != AttackResultType.Blocked)
                executor.GetWorldView().PlayTakeDamage(result);
            else
                executor.GetWorldView().PlayBlocked();
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
            _endTimer = _swingTimer * 2;
            executor.GetWorldView().PlayBlockPrep(1 / _swingTimer);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            _endTimer -= dt;
            if (_swingTimer > 0) return;
            stateEnum = UnitStateEnum.Block;
            if (_endTimer > 0) return;
            Finish();
        }

        public override void Finish()
        {
            base.Finish();
            base.executor.onTakeDamage -= OnTakeDamage; 
        }
    }
}