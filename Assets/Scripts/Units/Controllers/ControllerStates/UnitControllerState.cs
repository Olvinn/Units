using System;
using Units.Health;

namespace Units.Controllers.ControllerStates
{
    public abstract class UnitControllerState : IDisposable
    {
        public UnitStateEnum stateEnum { get; protected set; }
        
        public event Action onDone;
        
        protected IUnitStateMachine executor { get; set; }
        
        protected bool isActive { get; private set; }

        public virtual void Do() => isActive = true;

        public virtual void Finish()
        {
            if (!isActive) return;
            isActive = false;
            onDone?.Invoke();
        }

        public virtual void FinishSilent()
        {
            if (!isActive) return;
            isActive = false;
        }

        public virtual void Update(float dt) { }

        public virtual bool CanAttack() => false;
        public virtual bool CanBlock() => false;
        public virtual bool CanEvade() => false;
        public virtual bool CanMove() => false;
        public virtual AttackOutcome TakeDamage(AttackData attack) => new AttackOutcome();

        public virtual void Dispose()
        {
            onDone = null;
        }
    }
}
