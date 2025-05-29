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
        UnitState state { get; }
        IUnitModel GetModel();
        IUnitWorldView GetWorldView();
        Vector3 GetPosition();
        bool CanAttack();
        void Attack(IUnitController target);
        void Block(Attack attack);
        void Evade(Attack attack);
        void NotifyOfIncomingAttack(Attack attack);
        AttackOutcome GetDamage(Attack attack);
        void Move(IUnitController destination);
        void Move(Vector3 destination);
        void Follow(IUnitController target);
        void Patrol(Vector3 position);
        void Protect(IUnitController target);
        void Protect(Vector3 position, float radius);
    }
}
