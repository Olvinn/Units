using System;
using System.Collections.Generic;
using Units.Classes;
using Units.Enums;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demos.FreeSpace.Scripts
{
    public class BotBehaviour : IDisposable, IUnitBehaviour
    {
        private IUnitController _controller;
        
        private LinkedList<Attack> _incomingAttacks;
        private IEnumerable<IUnitController> _attackers;
        private IUnitBehaviour _target;
        
        public void Initialize(IUnitController controller)
        {
            _controller = controller;
            _incomingAttacks = new LinkedList<Attack>();

            _controller.onTakeDamage += ReactOnTakeDamage;
            _controller.onGetAttacked += ReactOnGetAttacked;
            
            WorldUnits.RegisterUnit(this);
        }

        public void Update(float dt)
        {
            _controller.Update(dt);
            
            if (_controller.state != UnitStateEnum.Idle)
                return;
            
            ReactOnAttackUpdate();

            CheckSurroundings();
        }

        public IUnitController GetController() => _controller;

        private void CheckSurroundings()
        {
            _target = WorldUnits.GetPotentialTarget(this);
            if (_controller.state == UnitStateEnum.Idle)
                _controller.Attack(_target.GetController());
        }

        private void ReactOnTakeDamage(AttackOutcome attack)
        {
            //Panic();
        }

        private void ReactOnGetAttacked(Attack attack)
        {
            if (_incomingAttacks.Count == 0)
            {
                _incomingAttacks.AddLast(attack);
                return;
            }
            var node = GetIncomingAttackNodeRightBefore(attack);
            _incomingAttacks.AddAfter(node, attack);

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
            else if (timeBeforeAttack > _controller.GetModel().GetTimeToBlock() && Random.value > .5f)
            {
                _controller.Block(attack);
            }
            else
            {
                _controller.Evade(attack);
            }
        }

        private LinkedListNode<Attack> GetIncomingAttackNodeRightBefore(Attack attack)
        {
            LinkedListNode<Attack> node = _incomingAttacks.First;
            while (node.Value.ApproxHitTime < attack.ApproxHitTime && node != _incomingAttacks.Last)
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