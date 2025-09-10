namespace Stages
{
    public abstract class BaseStage : IStage
    {
        protected bool isOpen { get; private set; }
        protected IStage previousStage { get; private set; }
        
        public virtual void Open()
        {
            if (isOpen) return;
            
            isOpen = true;
        }

        public virtual void Open(IStage previousStage)
        {
            if (isOpen) return;
            
            isOpen = true;
            if (previousStage != null) 
                previousStage.Close();
            this.previousStage = previousStage;
        }

        public virtual void Close()
        {
            if (!isOpen) return;
            
            isOpen = false;
            if (previousStage != null) 
                previousStage.Open();
        }

        public virtual void Update(float dt)
        {
            if (!isOpen) return;
        }
    }
}
