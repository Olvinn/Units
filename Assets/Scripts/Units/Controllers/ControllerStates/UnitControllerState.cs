using System;

namespace Units.Controllers.ControllerStates
{
    public abstract class UnitControllerState
    {
        public UnitStateEnum stateEnum { get; protected set; }
        
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
