using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitMovement : IUpdatable
    {
        public void Move(Vector3 destination);
    }
}