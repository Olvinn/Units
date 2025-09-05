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
        private BotBehaviour _redBot, _blueBot;
        
        void Start()
        {
            _redModel = UnitFactory.CreateRandomUnit(10, "Red");
            _blueModel = UnitFactory.CreateRandomUnit(10, "Blue");

            var redMovement = new UnitMovement(_redModel.GetStats().Speed, redWorldView.transform);
            var blueMovement = new UnitMovement(_blueModel.GetStats().Speed, blueWorldView.transform);

            _redController = new UnitController(_redModel, redMovement, null, redWorldView, new []{ _redView as IUnitUIView }); 
            _blueController = new UnitController(_blueModel, blueMovement, null, blueWorldView, new []{ _blueView as IUnitUIView });

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
