using UnityEngine;

namespace Units.Views
{
    public interface IUnitWorldView : IUnitView
    {
        void Play(Cue cue);
        void Play(Cue cue, float speed);
        Transform GetTransform();
    }
}
