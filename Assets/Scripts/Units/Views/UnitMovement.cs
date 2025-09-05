using System;
using UnityEngine;

namespace Units.Views
{
    public class UnitMovement : IUnitMovement
    {
        public bool IsMoving { get; private set; }
        public event Action onReachDestination;

        private float _speed, _stopDistance;
        private Transform _transform;
        
        private Vector3 _destination;

        public UnitMovement(float speed, Transform transform, float stopDistance)
        {
            _speed = speed;
            _transform = transform;
            _stopDistance = stopDistance;
        }
        
        public void Update(float dt)
        {
            if (IsMoving)
            {
                if (Vector3.Distance(_transform.position, _destination) <= _stopDistance)
                {
                    IsMoving = false;
                    onReachDestination?.Invoke();
                }
                else
                {
                    var dir = Vector3.Normalize(_destination - _transform.position);
                    _transform.position += dir * (_speed * dt);
                }
            }
        }

        public void Move(Vector3 destination)
        {
            _destination = destination;
            IsMoving = true;
        }

        public void Stop()
        {
            IsMoving = false;
        }
    }
}
