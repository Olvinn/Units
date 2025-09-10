namespace Stages
{
    public interface IStage
    {
        void Open();
        void Open(IStage previousStage);
        void Close();
        void Update(float dt);
    }
}
