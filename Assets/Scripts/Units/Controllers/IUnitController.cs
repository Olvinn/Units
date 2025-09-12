using System;
using Units.Controllers.ControllerStates;
using Units.Health;
using Units.Models;
using Units.Views;
using UnityEngine;

namespace Units.Controllers
{
    public interface IUnitController : IUpdatable
    {
        event Action<AttackData> onGetAttacked;
        event Action<AttackOutcome> onTakeDamage;
        UnitStateEnum state { get; }
        IUnitModel GetModel();
        IUnitWorldView GetWorldView();
        IUnitMovement GetMovement();
        Transform GetTransform();
        void Attack(IUnitController target);
        void Block(AttackData attackData);
        void Evade(AttackData attackData);
        void NotifyOfIncomingAttack(AttackData attackData);
        AttackOutcome TakeDamage(AttackData attackData);
        void Move(IUnitController target);
        void Move(Vector3 destination);
        void Do(UnitControllerState job);
    }
}
