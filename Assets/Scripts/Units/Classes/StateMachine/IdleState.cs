using Units.Enums;

namespace Units.Classes.StateMachine
{
    public class IdleState : UnitControllerState
    {
        public IdleState()
        {
            state = UnitState.Idle;
        }
    }
}