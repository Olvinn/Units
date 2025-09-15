using System;
using System.Collections.Generic;
using Units.Controllers;
using Units.Health;
using UnityEngine;

namespace UnitBehaviours
{
    public class BotBehaviour : IDisposable, IUnitBehaviour
    {
        public event Action onCheckSurroundings;
        
        private IUnitController _controller;
        
        private LinkedList<AttackData> _incomingAttacks;
        private IEnumerable<IUnitController> _attackers;
        private IUnitController _target;
        
        public BotBehaviour(IUnitController controller)
        {
            _controller = controller;
            _incomingAttacks = new LinkedList<AttackData>();

            _controller.onTakeDamage += ReactOnTakeDamage;
            _controller.onGetAttacked += ReactOnGetAttacked;
        }

        public void Update(float dt)
        {
            _controller?.Update(dt);
            
            ReactOnAttackUpdate();
            CheckSurroundings();
            
            if (_controller == null || _controller.state != UnitStateEnum.Idle || _target == null)
                return;
            
            _controller.Attack(_target);
        }
        
        public void SetKnownEnemies(List<IUnitBehaviour> enemies)
        {
            //Something something
        }

        public IUnitController GetController() => _controller;
        
        public void Attack(IUnitBehaviour unit)
        {
            _target = unit.GetController();
            _controller.Attack(unit.GetController());
        }
        
        public void MoveTo(Vector3 position)
        {
            _target = null;
            _controller.Move(position);
        }

        private void CheckSurroundings()
        {
            onCheckSurroundings?.Invoke();
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

            if (_target == null)
                _target = attack.Source;
            
            var timeBeforeAttack = _incomingAttacks.First.Value.ApproxHitTime - Time.time;

            if (timeBeforeAttack > _controller.GetModel().GetSwingTime())
                _controller.Attack(attack.Source);
            else if (timeBeforeAttack > _controller.GetModel().GetTimeToBlock())
                _controller.Block(attack);
            else
                _controller.Evade(attack);
        }

        private LinkedListNode<AttackData> GetIncomingAttackNodeRightBefore(AttackData attackData)
        {
            LinkedListNode<AttackData> node = _incomingAttacks.First;
            while (node.Value.ApproxHitTime < attackData.ApproxHitTime && node != _incomingAttacks.Last)
                node = node.Next ?? _incomingAttacks.Last;
            return node;
        }

        public void Dispose()
        {
            _controller.onTakeDamage -= ReactOnTakeDamage;
            _controller.onGetAttacked -= ReactOnGetAttacked;
            onCheckSurroundings = null;
        }
    }
}