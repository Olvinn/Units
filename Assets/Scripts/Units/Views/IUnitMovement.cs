using System;
using UnityEngine;

namespace Units.Views
{
    public interface IUnitMovement : IUpdatable
    {
        bool IsMoving { get; }
        event Action onReachDestination;
        public void Move(Vector3 destination, float stopDistance);
        public void Move(Transform target, float stopDistance);
        public void Stop();
    }
}