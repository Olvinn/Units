using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitWorldView : IUnitView
    {
        void PlayAttackPrep();
        void PlayAttack();
        Vector3 GetPosition();
    }
}
