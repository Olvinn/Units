using Units.Enums;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace Units.Classes.StateMachine
{
    public class EvadeState : UnitControllerState
    {
        private float _swingTimer;
        private Attack _attack;
        
        public EvadeState(IUnitController executor, Attack attack)
        {
            base.executor = executor;
            base.executor.onTakeDamage += OnTakeDamage; 
            _attack = attack;
            stateEnum = UnitStateEnum.BlockPrep;
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
            _swingTimer *= 2;
        }

        public override void Update(float dt)
        {
            if (!isActive) return;
            _swingTimer -= dt;
            if (_swingTimer > 0) return;
            Finish();
        }
    }
}