using Units.Enums;

namespace Units.Classes.StateMachine
{
    public class IdleState : UnitControllerState
    {
        public IdleState()
        {
            stateEnum = UnitStateEnum.Idle;
        }
    }
}