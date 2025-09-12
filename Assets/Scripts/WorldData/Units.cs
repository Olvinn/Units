using System.Collections.Generic;
using UnitBehaviours;
using Units.Controllers;
using Units.Views;

namespace WorldData
{
    public class Units
    {
        private static Units _instance;
        public static Units instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Units();
                return _instance;
            }
            private set => _instance = value;
        }

        private Dictionary<IUnitWorldView, IUnitBehaviour> _worldViews;
        private List<IUnitBehaviour> _allUnits;
        private List<IUnitBehaviour> _manualUnits;

        private Units()
        {
            _worldViews = new Dictionary<IUnitWorldView, IUnitBehaviour>();
            _allUnits = new List<IUnitBehaviour>();
            _manualUnits = new List<IUnitBehaviour>();
        }

        public void Update(float dt)
        {
            foreach (var unit in _allUnits)
                unit.Update(dt);
        }
        
        public void RegisterUnit(IUnitBehaviour unit)
        {
            if (unit == null) return;
            _allUnits.Add(unit);
            unit.onCheckSurroundings += () => GiveDebugEnemies(unit);
            IUnitController controller = unit.GetController();
            IUnitWorldView view = null;
            if (controller != null) view = controller.GetWorldView();
            if (view != null) _worldViews.Add(view, unit);
        }

        private void GiveDebugEnemies(IUnitBehaviour current)
        {
            foreach (var unit in _allUnits)
            {
                if (current == unit) continue;
                current.SetKnownEnemies(new List<IUnitBehaviour>() { unit });
            }
        }

        public void ResetManualUnits()
        {
            foreach (var unit in _manualUnits)
            {
                var view = unit.GetController().GetWorldView();
                
                _worldViews.Remove(view);
                _allUnits.Remove(unit);

                BotBehaviour bot = new BotBehaviour(unit.GetController());
            
                _worldViews.Add(view, bot);
                _allUnits.Add(bot);
            }
            _manualUnits.Clear();
        }

        public void SwitchToManual(IUnitWorldView unit)
        {
            if (!_worldViews.TryGetValue(unit, out var selected))
                return;
            
            var view = selected.GetController().GetWorldView();

            _worldViews.Remove(view);
            _allUnits.Remove(selected);

            ManualBehaviour manual = new ManualBehaviour(selected.GetController());
            
            _worldViews.Add(view, manual);
            _allUnits.Add(manual);
            _manualUnits.Add(manual);
        }

        public List<IUnitBehaviour> GetManualUnits()
        {
            List<IUnitBehaviour> result = new List<IUnitBehaviour>();
            foreach (var manualUnit in _manualUnits)
                result.Add(manualUnit);
            return result;
        }

        public IUnitBehaviour GetBehaviour(IUnitWorldView unit)
        {
            return _worldViews.GetValueOrDefault(unit);
        }
    }
}
