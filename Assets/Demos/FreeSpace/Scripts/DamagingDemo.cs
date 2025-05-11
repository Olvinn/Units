using Demos.FreeSpace.Scripts;
using Units.Classes;
using Units.Interfaces;
using UnityEngine;

namespace Demos
{
    public class DamagingDemo : MonoBehaviour
    {
        [SerializeField] private UnitViewPanel _redView;
        [SerializeField] private UnitViewPanel _blueView;

        [SerializeField] private UnitWorldView redWorldView;
        [SerializeField] private UnitWorldView blueWorldView;

        private IUnitModel _redModel, _blueModel;
        private IUnitController _red, _blue;
        
        void Start()
        {
            _redModel = UnitFactory.CreateRandomUnit(10, "Red");
            _blueModel = UnitFactory.CreateRandomUnit(10, "Blue");

            _red = new PassiveUnitController(_redModel, null, null, redWorldView, new []{ _redView as IUnitUIView }); 
            _blue = new PassiveUnitController(_blueModel, null, null, blueWorldView, new []{ _blueView as IUnitUIView }); 
            
            _red.Attack(_blue);
        }

        private void Update()
        {
            _red.Update(Time.deltaTime);
            _blue.Update(Time.deltaTime);
        }
    }
}
