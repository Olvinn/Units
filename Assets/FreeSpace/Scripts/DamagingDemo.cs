using FreeSpace.Scripts;
using Units.Behaviour;
using Units.Interfaces;
using UnityEngine;

namespace Demos.FreeSpace.Scripts
{
    public class DamagingDemo : MonoBehaviour
    {
        [SerializeField] private UnitViewPanel _redView;
        [SerializeField] private UnitViewPanel _blueView;

        [SerializeField] private UnitWorldView redWorldView;
        [SerializeField] private UnitWorldView blueWorldView;

        private IUnitModel _redModel, _blueModel;
        private IUnitController _redController, _blueController;
        private BotBehaviour _redBot, _blueBot;
        
        void Start()
        {
            _redModel = UnitFactory.CreateRandomUnit(10, "Red");
            _blueModel = UnitFactory.CreateRandomUnit(10, "Blue");

            _redController = new UnitController(_redModel, null, null, redWorldView, new []{ _redView as IUnitUIView }); 
            _blueController = new UnitController(_blueModel, null, null, blueWorldView, new []{ _blueView as IUnitUIView });

            _redBot = new BotBehaviour();
            _redBot.Initialize(_redController);

            _blueBot = new BotBehaviour();
            _blueBot.Initialize(_blueController);
        }

        private void FixedUpdate()
        {
            if (Random.value > .5f)
            {
                _redBot.Update(Time.fixedDeltaTime);
                _blueBot.Update(Time.fixedDeltaTime); 
            }
            else
            {
                _blueBot.Update(Time.fixedDeltaTime);
                _redBot.Update(Time.fixedDeltaTime);
            }
        }
    }
}
