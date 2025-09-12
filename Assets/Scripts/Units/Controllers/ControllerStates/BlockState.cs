using Units.Health;
using Units.Views;
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
            base.executor.onTakeDamage -= OnTakeDamage; 
            if (result.ResultType != AttackResultType.Blocked)
                executor.GetWorldView().PlayTakeDamage(result);
            else
                executor.GetWorldView().Play(Cue.Block);
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
            executor.GetWorldView().Play(Cue.BlockPreparation, 1 / _swingTimer);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            _endTimer -= dt;
            if (_swingTimer > 0) return;
            stateEnum = UnitStateEnum.Blocking;
            if (_endTimer > 0) return;
            Finish();
        }

        public override void Finish()
        {
            base.Dispose();
            base.executor.onTakeDamage -= OnTakeDamage; 
        }

        public override void FinishSilent()
        {
            base.Dispose();
            base.executor.onTakeDamage -= OnTakeDamage; 
        }
    }
}