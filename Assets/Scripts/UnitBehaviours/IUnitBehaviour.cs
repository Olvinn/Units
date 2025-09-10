using Units.Controllers;
using UnityEngine;

namespace UnitBehaviours
{
    public interface IUnitBehaviour
    {
        IUnitController GetController();
        void Attack(IUnitBehaviour unit);
        void MoveTo(Vector3 position);
    }
}