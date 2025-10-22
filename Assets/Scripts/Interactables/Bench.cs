using UnitBehaviours;
using Units.Controllers.ControllerStates;
using UnityEngine;

namespace Interactables
{
    public class Bench : MonoBehaviour, IInteractable
    {
        
        
        public void Interact(IUnitBehaviour unit)
        {
            MovingState job = new MovingState(unit.GetStateMachine(), transform.position);
            unit.GetController().Do(job);
        }
    }
}
