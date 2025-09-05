using System.Collections.Generic;
using Units.Health;

namespace Units.Views
{
    /// <summary>
    /// This is unit's state container commonly used for communicate with UIs
    /// </summary>
    public struct UnitStateContainer
    {
        public string UnitName;
        public string Attributes, Stats;
        public Dictionary<LimbType, float> LimbState;
        public string Status;
    }
}
