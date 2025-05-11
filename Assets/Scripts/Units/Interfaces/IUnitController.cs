using Units.Structures;
using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitController : IDamageable, IUpdatable
    {
        IUnitModel GetModel();
        Vector3 GetPosition();
        bool CanAttack();
        void Attack(IUnitController target);
        void NotifyOfIncomingAttack(Attack attack);
        void Move(IUnitController destination);
        void Move(Vector3 destination);
        void Follow(IUnitController target);
        void Patrol(Vector3 position);
        void Protect(IUnitController target);
        void Protect(Vector3 position, float radius);
    }
}
