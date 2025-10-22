using Units.Health;
using Units.Views;
using UnityEngine;

namespace Units.Controllers.ControllerStates
{
    public class BlockState : UnitControllerState
    {
        private enum Phase { Preparing, Blocking }
        
        private float _prepareTimer;
        private AttackData _attackData;
        private Phase _phase;
        private bool _attackBlocked;
        
        public BlockState(IUnitStateMachine executor, AttackData attackData)
        {
            base.executor = executor;
            _attackData = attackData;
            stateEnum = UnitStateEnum.BlockPrep;
        }

        public override void Do()
        {
            base.Do();
            if (executor == null)
            {
                Finish();
                return;
            }
            _prepareTimer = executor.GetModel().GetTimeToBlock();
            executor.GetWorldView().Play(Cue.BlockPreparation, 1 / _prepareTimer);
            _phase = Phase.Preparing;
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            switch (_phase)
            {
                case Phase.Preparing:
                    _prepareTimer -= dt;
                    if (_prepareTimer < 0)
                        _phase = Phase.Blocking;
                    break;
                case Phase.Blocking:
                    if (_attackBlocked)
                        Finish();
                    break;
            }
        }

        public override void Finish()
        {
            base.Dispose();
        }

        public override void FinishSilent() => Finish();

        public override bool CanAttack() => true;
        public override bool CanEvade() => true;
        public override bool CanMove() => true;
        
        public override AttackOutcome TakeDamage(AttackData attack)
        {
            _attackBlocked = true;
            var result = _phase == Phase.Blocking ? executor.GetModel().TryBlockDamage(attack) :
                executor.GetModel().GetDamage(attack);
            if (result.ResultType != AttackResultType.Blocked)
                executor.GetWorldView().PlayTakeDamage(result);
            else
                executor.GetWorldView().Play(Cue.Block);
            return result;
        }
    }
}