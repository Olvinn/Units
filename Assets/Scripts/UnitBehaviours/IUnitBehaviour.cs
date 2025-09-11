using System;
using System.Collections.Generic;
using Units.Controllers;
using UnityEngine;

namespace UnitBehaviours
{
    public interface IUnitBehaviour
    {
        event Action onCheckSurroundings;
        void SetKnownEnemies(List<IUnitBehaviour> enemies);
        IUnitController GetController();
        void Attack(IUnitBehaviour unit);
        void MoveTo(Vector3 position);
        void SetManualControl(bool value);
    }
}