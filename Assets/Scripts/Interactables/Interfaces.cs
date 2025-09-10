using System;
using UnitBehaviours;

namespace Interactables
{
    public interface IInteractable
    {
        void Interact(IUnitBehaviour unit);
    }
    
    public interface IBench : IInteractable
    {
        event Action onDone;
        bool isOccupied { get; }
        void Occupy(IUnitBehaviour unit);
        IUnitBehaviour GetOccupier();
        IUnitBehaviour Stop();
        IJob GetJob();
    }

    public interface IJob
    {
        void Do(IUnitBehaviour unit);
    }
}
