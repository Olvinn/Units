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
        private Transform _target;

        public UnitMovement(float speed, Transform transform)
        {
            _speed = speed;
            _transform = transform;
        }
        
        public void Update(float dt)
        {
            if (IsMoving)
            {
                var dest = _target?.position ?? _destination;
                if (Vector3.Distance(_transform.position, dest) <= _stopDistance)
                {
                    IsMoving = false;
                    onReachDestination?.Invoke();
                }
                else
                {
                    var dir = Vector3.Normalize(dest - _transform.position);
                    _transform.position += dir * (_speed * dt);
                }
                var lookAt = dest;
                lookAt.y = _transform.position.y;
                _transform.LookAt(lookAt);
            }
        }

        public void Move(Vector3 destination, float stopDistance)
        {
            _target = null;
            _destination = destination;
            _stopDistance = stopDistance;
            IsMoving = true;
        }

        public void Move(Transform target, float stopDistance)
        {
            _target = target;
            _stopDistance = stopDistance;
            IsMoving = true;
        }

        public void Stop()
        {
            onReachDestination = null;
            IsMoving = false;
        }
    }
}
