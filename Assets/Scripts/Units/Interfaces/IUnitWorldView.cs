using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitWorldView : IUnitView
    {
        void PlayAttackPrep(float speed);
        void PlayAttack();
        Vector3 GetPosition();
    }
}
