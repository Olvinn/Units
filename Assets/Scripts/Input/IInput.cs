using System;
using UnityEngine;

namespace Input
{
    public interface IInput
    {
        Vector3 mousePosition { get; }
        event Action onLeftClick;
        event Action onRightClick;
    }
}
