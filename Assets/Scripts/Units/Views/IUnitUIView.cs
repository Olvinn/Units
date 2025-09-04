using Units.Controllers;
using UnityEngine;

namespace Units.Views
{
    public interface IUnitUIView : IUnitView
    {
        void ShowNotification(string message, Vector3 unitWorldPos);
        void UpdateView(UnitStateContainer state);
    }
}
