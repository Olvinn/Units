using System.Collections.Generic;
using Input;
using Interactables;
using UnitBehaviours;
using Units.Controllers.ControllerStates;
using Units.Views;
using UnityEngine;

namespace Stages
{
    public class RTSStage : BaseStage
    {
        private IInput _input; 
        private Camera _camera;
        
        public RTSStage(IInput input, Camera camera)
        {
            _input = input;
            _camera = camera;
        }

        public override void Open()
        {
            if (isOpen)  return;
            base.Open();
            SubscribeOnInput();
        }

        public override void Open(IStage previousStage)
        {
            if (isOpen)  return;
            base.Open(previousStage);
            SubscribeOnInput();
        }

        public override void Close()
        {
            if (!isOpen) return;
            base.Close();
            UnsubscribeFromInput();
        }

        private void SubscribeOnInput()
        {
            _input.onLeftClick += RaycastForSelectable;
            _input.onRightClick += RaycastForTarget;
        }

        private void UnsubscribeFromInput()
        {
            _input.onLeftClick -= RaycastForSelectable;
            _input.onRightClick -= RaycastForTarget;
        }

        private void RaycastForSelectable()
        {
            WorldData.Units.instance.ResetManualUnits();
            Ray ray = _camera.ScreenPointToRay(_input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                IUnitWorldView unit = hit.collider.GetComponent<IUnitWorldView>();
                if (unit != null)
                    WorldData.Units.instance.SwitchToManual(unit);
            }
        }

        private void RaycastForTarget()
        {
            List<IUnitBehaviour> units = WorldData.Units.instance.GetManualUnits();
            
            Ray ray = _camera.ScreenPointToRay(_input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                IUnitWorldView unit = hit.collider.GetComponent<IUnitWorldView>();
                IInteractable interactible = hit.collider.GetComponent<IInteractable>();
                if (unit != null)
                {
                    foreach (var controllableUnit in units)
                        controllableUnit.Attack(WorldData.Units.instance.GetBehaviour(unit));
                }
                else if (interactible != null)
                {
                    foreach (var controllableUnit in units)
                    {
                        interactible.Interact(controllableUnit);
                    }
                }
                else
                {
                    foreach (var controllableUnit in units)
                        controllableUnit.MoveTo(hit.point);
                }
            }
        }
    }
}
