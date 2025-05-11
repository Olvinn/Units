using System.Collections.Generic;
using Units.Interfaces;
using Units.Structures;

namespace Units.Classes
{
    public static class WorldUnits
    {
        public static IEnumerable<IUnitController> GetTargetsAffectedByAttack(IUnitController source, IUnitController target, Attack attack)
        {
            return new [] { target };
        }
    }
}
