using System;
using System.Collections.Generic;
using Units.Controllers;
using Units.Health;
using UnityEngine;

namespace UnitBehaviours
{
    public class BotBehaviour : IDisposable, IUnitBehaviour
    {
        private IUnitController _controller;
        private IUnitSense _sense;
        
        private LinkedList<AttackData> _incomingAttacks;
        private IEnumerable<IUnitController> _attackers;
        private IUnitBehaviour _target;
        
        public BotBehaviour(IUnitController controller, IUnitSense sense)
        {
            _controller = controller;
            _sense = sense;
            _incomingAttacks = new LinkedList<AttackData>();

            _controller.onTakeDamage += ReactOnTakeDamage;
            _controller.onGetAttacked += ReactOnGetAttacked;
            
            WorldUnits.RegisterUnit(this);
        }

        public void Update(float dt)
        {
            _controller?.Update(dt);
            _sense?.Update(dt);
            
            if (_controller == null || _controller.state != UnitStateEnum.Idle)
                return;
            
            ReactOnAttackUpdate();
            CheckSurroundings();
        }

        public IUnitController GetController() => _controller;
        
        public void Attack(IUnitBehaviour unit)
        {
            _controller.Attack(_target.GetController());
        }

        public void MoveTo(Vector3 position)
        {
            _controller.Move(position);
        }

        private void CheckSurroundings()
        {
            _target = WorldUnits.GetPotentialTarget(this);
            _controller.Attack(_target.GetController());
        }

        private void ReactOnTakeDamage(AttackOutcome attack)
        {
            //Panic();
        }

        private void ReactOnGetAttacked(AttackData attackData)
        {
            if (_incomingAttacks.Count == 0)
            {
                _incomingAttacks.AddLast(attackData);
                return;
            }
            var node = GetIncomingAttackNodeRightBefore(attackData);
            _incomingAttacks.AddAfter(node, attackData);

            ReactOnAttackUpdate();
        }

        private void ReactOnAttackUpdate()
        {
            if (_incomingAttacks.Count == 0) return;
            if (_incomingAttacks.First.Value.ApproxHitTime < Time.time)
            {
                _incomingAttacks.RemoveFirst();
                if (_incomingAttacks.Count == 0) return;
            }

            var attack = _incomingAttacks.First.Value;
            var timeBeforeAttack = _incomingAttacks.First.Value.ApproxHitTime - Time.time;

            if (timeBeforeAttack > _controller.GetModel().GetSwingTime())
            {
                _controller.Attack(attack.Source);
            }
            else if (timeBeforeAttack > _controller.GetModel().GetTimeToBlock())
            {
                _controller.Block(attack);
            }
            else
            {
                _controller.Evade(attack);
            }
        }

        private LinkedListNode<AttackData> GetIncomingAttackNodeRightBefore(AttackData attackData)
        {
            LinkedListNode<AttackData> node = _incomingAttacks.First;
            while (node.Value.ApproxHitTime < attackData.ApproxHitTime && node != _incomingAttacks.Last)
            {
                node = node.Next ?? _incomingAttacks.Last;
            }
            return node;
        }

        public void Dispose()
        {
            _controller.onTakeDamage -= ReactOnTakeDamage;
            _controller.onGetAttacked -= ReactOnGetAttacked;
        }
    }
}