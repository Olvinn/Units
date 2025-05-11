using Units.Classes;
using Units.Interfaces;

public class PassiveUnitController : BaseUnitController
{
    public PassiveUnitController(IUnitModel model, IUnitMovement movement, IUnitSense sense, IUnitWorldView worldView, IUnitUIView[] _uiViews) : 
        base(model, movement, sense, worldView, _uiViews)
    {
        
    }
}
