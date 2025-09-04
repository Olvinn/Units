using Units.Behaviour;
using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitUIView : IUnitView
    {
        void ShowNotification(string message, Vector3 unitWorldPos);
        void UpdateView(UnitStateContainer state);
    }
}
