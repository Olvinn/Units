using Units.Enums;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace Units.Classes.StateMachine
{
    public class BlockState : UnitControllerState
    {
        private float _swingTimer, _endTimer;
        private Attack _attack;
        
        public BlockState(IUnitController executor, Attack attack)
        {
            base.executor = executor;
            base.executor.onTakeDamage += OnTakeDamage; 
            _attack = attack;
            state = UnitState.BlockPrep;
        }

        private void OnTakeDamage(AttackOutcome result)
        {
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
            _swingTimer = _attack.ApproxHitTime - Time.time;
            _endTimer = _swingTimer * 2;
            executor.GetWorldView().PlayBlockPrep(1 / _swingTimer);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            _endTimer -= dt;
            if (_swingTimer > 0) return;
            state = UnitState.Block;
            if (_endTimer > 0) return;
            Finish();
        }
    }
}