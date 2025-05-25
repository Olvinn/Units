using UnityEngine;

namespace Units.Interfaces
{
    public interface IUnitUIView : IUnitView
    {
        void ShowNotification(string message, Vector3 unitWorldPos);
        void UpdateView(string name, string stats);
    }
}
