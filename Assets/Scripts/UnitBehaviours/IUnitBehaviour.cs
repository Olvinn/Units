using System;
using System.Collections.Generic;
using Units;
using Units.Controllers;
using UnityEngine;

namespace UnitBehaviours
{
    public interface IUnitBehaviour: IUpdatable
    {
        event Action onCheckSurroundings;
        void SetKnownEnemies(List<IUnitBehaviour> enemies);
        IUnitController GetController();
        void Attack(IUnitBehaviour unit);
        void MoveTo(Vector3 position);
    }
}