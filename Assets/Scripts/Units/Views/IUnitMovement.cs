using UnityEngine;

namespace Units.Views
{
    public interface IUnitMovement : IUpdatable
    {
        public void Move(Vector3 destination);
    }
}