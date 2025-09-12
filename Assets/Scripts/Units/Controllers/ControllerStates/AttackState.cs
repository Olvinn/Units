using Units.Health;
using Units.Views;

namespace Units.Controllers.ControllerStates
{
    public class AttackState : UnitControllerState
    {
        private enum Phase { Mov, Swing, Attack, Rest }
        
        private IUnitController _target;
        private float _swingTimer, _finishTimer;
        private AttackData _attackData;
        private Phase _phase;
        
        public AttackState(IUnitController attacker, IUnitController target)
        {
            executor = attacker;
            _target = target;
            stateEnum = UnitStateEnum.Attack;
        }

        public override void Do()
        {
            base.Do();
            if (_target == null || executor == null)
            {
                Finish();
                return;
            }
            executor.GetMovement().onReachDestination += OnReachTarget; 
            executor.GetMovement().Move(_target.GetTransform(), executor.GetModel().GetStats().AttackDistance);
            _phase = Phase.Mov;
        }

        public override void Update(float dt)
        {
            switch (_phase)
            {
                case Phase.Mov:
                    break;
                case Phase.Swing:
                    _swingTimer -= dt;
                    if (_swingTimer > 0) break;
                    _phase = Phase.Attack;
                    break;
                case Phase.Attack:
                    _target.TakeDamage(_attackData);
                    executor.GetWorldView().Play(Cue.Attack);
                    _phase = Phase.Rest;
                    break;
                case Phase.Rest:
                    _finishTimer -= dt;
                    if (_finishTimer > 0) break;
                    Finish();
                    break;
            }
        }

        private void OnReachTarget()
        {
            executor.GetMovement().Stop();
            _swingTimer = executor.GetModel().GetSwingTime();
            _finishTimer = 1.5f;
            executor.GetWorldView().Play(Cue.AttackPreparation, 1 / _swingTimer);
            _attackData = executor.GetModel().GetAttack();
            _attackData.Source = executor;
            _target.NotifyOfIncomingAttack(_attackData);
            _phase = Phase.Swing;
        }

        public override void Finish()
        {
            executor.GetMovement().onReachDestination -= OnReachTarget; 
            base.Finish();
        }

        public override void FinishSilent()
        {
            executor.GetMovement().onReachDestination -= OnReachTarget; 
            base.Dispose();
        }
    }
}