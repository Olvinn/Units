using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitWorldView : IUnitView
    {
        void PlayAttack();
        Vector3 GetPosition();
    }
}
