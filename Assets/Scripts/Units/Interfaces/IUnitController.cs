using System;
using Units.Behaviour;
using Units.Health;
using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitController : IUpdatable
    {
        event Action<AttackData> onGetAttacked;
        event Action<AttackOutcome> onTakeDamage;
        UnitStateEnum state { get; }
        IUnitModel GetModel();
        IUnitWorldView GetWorldView();
        Vector3 GetPosition();
        void Attack(IUnitController target);
        void Block(AttackData attackData);
        void Evade(AttackData attackData);
        void NotifyOfIncomingAttack(AttackData attackData);
        AttackOutcome TakeDamage(AttackData attackData);
        void Move(IUnitController destination);
        void Move(Vector3 destination);
    }
}
