using System;
using Units.Enums;
using Units.Interfaces;

namespace Units.Classes.StateMachine
{
    public abstract class UnitControllerState
    {
        public UnitState state { get; protected set; }
        
        public event Action onDone;
        
        protected IUnitController executor { get; set; }
        
        protected bool isActive { get; private set; }

        public virtual void Do() => isActive = true;

        public void Finish()
        {
            isActive = false;
            onDone?.Invoke();
        }

        public virtual void Update(float dt) { }
    }
}
