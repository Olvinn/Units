using System;
using Units.Enums;
using Units.Structures;
using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitController : IUpdatable
    {
        event Action<Attack> onGetAttacked;
        event Action<AttackOutcome> onTakeDamage;
        UnitStateEnum state { get; }
        IUnitModel GetModel();
        IUnitWorldView GetWorldView();
        Vector3 GetPosition();
        void Attack(IUnitController target);
        void Block(Attack attack);
        void Evade(Attack attack);
        void NotifyOfIncomingAttack(Attack attack);
        AttackOutcome TakeDamage(Attack attack);
        void Move(IUnitController destination);
        void Move(Vector3 destination);
    }
}
