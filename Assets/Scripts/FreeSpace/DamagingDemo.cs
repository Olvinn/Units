using Units.Behaviours;
using Units.Controllers;
using Units.Models;
using Units.Views;
using UnityEngine;

namespace FreeSpace
{
    public class DamagingDemo : MonoBehaviour
    {
        [SerializeField] private UnitViewPanel _redView;
        [SerializeField] private UnitViewPanel _blueView;

        [SerializeField] private UnitWorldView redWorldView;
        [SerializeField] private UnitWorldView blueWorldView;

        private IUnitModel _redModel, _blueModel;
        private IUnitController _redController, _blueController;
        private UnitBehaviour _redUnit, _blueUnit;
        
        void Start()
        {
            _redModel = UnitFactory.CreateRandomUnit(10, "Red");
            _blueModel = UnitFactory.CreateRandomUnit(10, "Blue");

            _redController = new UnitController(_redModel, null, null, redWorldView, new []{ _redView as IUnitUIView }); 
            _blueController = new UnitController(_blueModel, null, null, blueWorldView, new []{ _blueView as IUnitUIView });

            _redUnit = new UnitBehaviour();
            _redUnit.Initialize(_redController);

            _blueUnit = new UnitBehaviour();
            _blueUnit.Initialize(_blueController);
        }

        private void FixedUpdate()
        {
            if (Random.value > .5f)
            {
                _redUnit.Update(Time.fixedDeltaTime);
                _blueUnit.Update(Time.fixedDeltaTime); 
            }
            else
            {
                _blueUnit.Update(Time.fixedDeltaTime);
                _redUnit.Update(Time.fixedDeltaTime);
            }
        }
    }
}
