using Units.Health;
using Units.Models;
using Units.Views;

namespace Units.Controllers
{
    public interface IUnitStateMachine
    {
        AttackData GetAttack();
        IUnitModel GetModel();
        IUnitWorldView GetWorldView();
        IUnitMovement GetMovement();
    }
}