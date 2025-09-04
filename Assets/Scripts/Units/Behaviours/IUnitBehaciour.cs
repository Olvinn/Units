using Units.Controllers;

namespace Units.Behaviours
{
    public interface IUnitBehaviour
    {
        IUnitController GetController();
    }
}