using System.Collections.Generic;
using Units.Behaviours;
using Units.Health;

namespace Units.Controllers
{
    public static class WorldUnits
    {
        private static HashSet<IUnitBehaviour> _registeredUnits = new HashSet<IUnitBehaviour>();
        
        public static IEnumerable<IUnitController> GetTargetsAffectedByAttack(IUnitController source, IUnitController target, AttackData attackData)
        {
            return new [] { target };
        }

        public static void RegisterUnit(IUnitBehaviour unit)
        {
            _registeredUnits.Add(unit);
        }

        public static IUnitBehaviour GetPotentialTarget(IUnitBehaviour unitBehaviour)
        {
            foreach (var unit in _registeredUnits)
            {
                if (unitBehaviour != unit)
                    return unit;
            }

            return null;
        }
    }
}
