using UnityEngine;

namespace Units.Views
{
    public interface IUnitWorldView : IUnitView
    {
        void PlayAttackPrep(float speed);
        void PlayBlockPrep(float speed);
        void PlayBlocked();
        void PlayEvasion();
        void PlayAttack();
        Transform GetTransform();
    }
}
