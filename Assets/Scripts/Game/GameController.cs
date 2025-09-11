using Game.UI;
using Input;
using Stages;
using UnitBehaviours;
using Units.Controllers;
using Units.Models;
using Units.Views;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private InputListener _input;
        
        [SerializeField] private UnitViewPanel _redView;
        [SerializeField] private UnitViewPanel _blueView;

        [SerializeField] private UnitWorldView redWorldView;
        [SerializeField] private UnitWorldView blueWorldView;

        private RTSStage _stage;
        private IUnitModel _redModel, _blueModel;
        private IUnitController _redController, _blueController;
        private BotBehaviour _redBot, _blueBot;
        
        void Start()
        {
            DebugStart();
        }

        private void FixedUpdate()
        {
            DebugUpdate(Time.fixedDeltaTime);
        }

        private void DebugStart()
        {
            _stage = new RTSStage(_input, Camera.main);
            _stage.Open();
            
            _redModel = UnitFactory.CreateRandomUnit(10, "Red");
            _blueModel = UnitFactory.CreateRandomUnit(10, "Blue");

            var redMovement = new UnitMovement(_redModel.GetStats().Speed, redWorldView.transform);
            var blueMovement = new UnitMovement(_blueModel.GetStats().Speed, blueWorldView.transform);

            _redController = new UnitController(_redModel, redMovement, redWorldView, new []{ _redView as IUnitUIView }); 
            _blueController = new UnitController(_blueModel, blueMovement, blueWorldView, new []{ _blueView as IUnitUIView });

            _redBot = new BotBehaviour(_redController);
            _blueBot = new BotBehaviour(_blueController);

            WorldData.Units.RegisterUnit(_redBot);
            WorldData.Units.RegisterUnit(_blueBot);
        }

        private void DebugUpdate(float dt)
        {
            _stage.Update(dt);
            if (Random.value > .5f)
            {
                _redBot.Update(dt);
                _blueBot.Update(dt); 
            }
            else
            {
                _blueBot.Update(dt);
                _redBot.Update(dt);
            }
        }
    }
}
