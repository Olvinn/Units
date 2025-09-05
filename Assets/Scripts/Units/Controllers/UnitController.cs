using System;
using Units.Controllers.ControllerStates;
using Units.Health;
using Units.Models;
using Units.Views;
using UnityEngine;

namespace Units.Controllers
{
    public class UnitController : IUnitController
    {
        public event Action<AttackData> onGetAttacked;
        public event Action<AttackOutcome> onTakeDamage;

        public UnitStateEnum state => _currentJob.stateEnum;
        
        private IUnitModel _model;
        private IUnitMovement _movement;
        private IUnitSense _sense;
        private IUnitWorldView _worldView;
        private IUnitUIView[] _uiViews;

        private UnitControllerState _currentJob;
    
        public UnitController(IUnitModel model, IUnitMovement movement, IUnitSense sense, IUnitWorldView worldView, IUnitUIView[] uiViews)
        {
            _model = model;
            _movement = movement;
            _sense = sense;
            _worldView = worldView;
            _uiViews = uiViews;
            _currentJob = new IdleState();
            _currentJob.Do();
        }

        public void Update(float dt)
        {
            _movement?.Update(dt);
            _sense?.Update(dt);
            _currentJob?.Update(dt);

            var data = _model.GetStateContainer();
            data.Status = state.ToString();

            foreach (var uiView in _uiViews)
                uiView.UpdateView(data);
        }
        
        public IUnitModel GetModel() => _model;
        
        public IUnitWorldView GetWorldView() => _worldView;
        public IUnitMovement GetMovement() => _movement;

        public Vector3 GetPosition() => _worldView.GetPosition();
        
        private bool CanAttack()
        {
            return (state == UnitStateEnum.Idle || state == UnitStateEnum.BlockPrep) 
                   && _model.CanAttack();
        }
        private bool CanBlock()
        {
            return (state == UnitStateEnum.Idle);
        }
        private bool CanEvade()
        {
            return (state == UnitStateEnum.Idle || state == UnitStateEnum.BlockPrep);
        }

        public void Attack(IUnitController target)
        {
            if (!CanAttack()) return;

            ChangeState(new AttackState(this, target));
        }

        public void Block(AttackData attackData)
        {
            if (!CanBlock()) return;
            
            ChangeState(new BlockState(this, attackData));
        }

        public void Evade(AttackData attackData)
        {
            if (!CanEvade()) return;
            
            ChangeState(new EvadeState(this, attackData));
        }

        public void NotifyOfIncomingAttack(AttackData attackData) => onGetAttacked?.Invoke(attackData);

        public virtual AttackOutcome TakeDamage(AttackData attackData)
        {
            AttackOutcome result;
            switch (state)
            {
                case UnitStateEnum.BlockPrep:
                    result = _model.TryBlockDamage(attackData);
                    break;
                case UnitStateEnum.Evading:
                    result = _model.TryEvadeDamage(attackData);
                    break;
                default:
                    result = _model.GetDamage(attackData);
                    break;
            }
            
            onTakeDamage?.Invoke(result);
            
            if (result.ResultType == AttackResultType.Full)
                ChangeState(new StaggeringState(this, result));
            else if (result.ResultType == AttackResultType.Partial)
                _worldView.PlayTakeDamage(result);
            
            foreach (var uiView in _uiViews)
            {
                uiView.PlayTakeDamage(result);
                uiView.ShowNotification($"{result.ResultType} : {result.HpChange:F1}", _worldView.GetPosition() + Vector3.up * 2.5f);
            }
            
            return result;
        }

        public void Move(IUnitController destination)
        {
            _movement.Move(destination.GetPosition());
        }

        public void Move(Vector3 destination)
        {
            _movement.Move(destination);
        }

        private void ChangeState(UnitControllerState newState)
        {
            _currentJob.Finish();
            _currentJob = newState;
            _currentJob.onDone += () =>
            {
                _currentJob = new IdleState();
            };
            _currentJob.Do();
        }
    }
}
