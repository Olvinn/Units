using System;

namespace Base
{
    public interface IPoolable
    {
        Action OnDone { get; }
    }
}
