
using Units.Views;
using UnityEngine;

namespace Units.Controllers.ControllerStates
{
    public class MovingState : UnitControllerState
    {
        private IUnitController _target;
        private Vector3 _destination;
        
        public MovingState(IUnitController executor, IUnitController target)
        {
            base.executor = executor;
            _target = target;
            stateEnum = UnitStateEnum.Moving;
        }
        
        public MovingState(IUnitController executor, Vector3 destination)
        {
            base.executor = executor;
            _destination = destination;
            stateEnum = UnitStateEnum.Moving;
        }

        public override void Do()
        {
            base.Do();
            if (executor == null)
            {
                Finish();
                return;
            }

            var stopDist = executor.GetModel().GetStats().AttackDistance;
            if (_target != null) 
                executor.GetMovement().Move(_target.GetTransform(), stopDist); 
            else 
                executor.GetMovement().Move(_destination, stopDist);
            executor.GetWorldView().Play(Cue.Idle);
            executor.GetMovement().onReachDestination += OnReachTarget; 
        }

        private void OnReachTarget()
        {
            Finish();
        }

        public override void Finish()
        {
            executor.GetMovement().onReachDestination -= OnReachTarget;
            if (executor != null && isActive)
                executor.GetMovement().Stop();
            base.Finish();
        }

        public override void FinishSilent()
        {
            executor.GetMovement().onReachDestination -= OnReachTarget;
            if (executor != null && isActive)
                executor.GetMovement().Stop();
            base.FinishSilent();
        }
    }
}