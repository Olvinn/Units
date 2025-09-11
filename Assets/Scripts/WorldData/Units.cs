using System;
using System.Collections.Generic;
using UnitBehaviours;
using Units.Controllers;
using Units.Views;

namespace WorldData
{
    public static class Units
    {
        private static Dictionary<IUnitWorldView, IUnitBehaviour> _worldViews;
        private static List<IUnitBehaviour> _allUnits;
        private static List<IUnitBehaviour> _manualUnits;

        static Units()
        {
            _worldViews = new Dictionary<IUnitWorldView, IUnitBehaviour>();
            _allUnits = new List<IUnitBehaviour>();
            _manualUnits = new List<IUnitBehaviour>();
        }
        
        public static void RegisterUnit(IUnitBehaviour unit)
        {
            if (unit == null) return;
            _allUnits.Add(unit);
            unit.onCheckSurroundings += () => GiveDebugEnemies(unit);
            IUnitController controller = unit.GetController();
            IUnitWorldView view = null;
            if (controller != null) view = controller.GetWorldView();
            if (view != null) _worldViews.Add(view, unit);
        }

        private static void GiveDebugEnemies(IUnitBehaviour current)
        {
            foreach (var unit in _allUnits)
            {
                if (current == unit) continue;
                current.SetKnownEnemies(new List<IUnitBehaviour>() { unit });
            }
        }

        public static void ResetManualUnits()
        {
            foreach (var unit in _manualUnits)
                unit.SetManualControl(false);
            _manualUnits.Clear();
        }

        public static void SwitchToManual(IUnitWorldView unit)
        {
            var selected = _worldViews[unit];
            selected.SetManualControl(true);
            _manualUnits.Add(selected);
        }

        public static List<IUnitBehaviour> GetManualUnits()
        {
            List<IUnitBehaviour> result = new List<IUnitBehaviour>();
            foreach (var manualUnit in _manualUnits)
                result.Add(manualUnit);
            return result;
        }

        public static IUnitBehaviour GetBehaviour(IUnitWorldView unit)
        {
            return  _worldViews[unit];
        }
    }
}
