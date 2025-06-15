using System;
using Units.Classes.StateMachine;
using Units.Enums;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace Units.Classes
{
    public class UnitController : IUnitController
    {
        public event Action<Attack> onGetAttacked;
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

        public virtual void Update(float dt)
        {
            _movement?.Update(dt);
            _sense?.Update(dt);
            _currentJob?.Update(dt);

            foreach (var uiView in _uiViews)
                uiView.UpdateView(_model.GetStateContainer());
        }
        
        public IUnitModel GetModel() => _model;
        
        public IUnitWorldView GetWorldView() => _worldView;

        public Vector3 GetPosition() => _worldView.GetPosition();
        
        public bool CanAttack()
        {
            return (state is UnitStateEnum.Idle or UnitStateEnum.Evading or UnitStateEnum.BlockPrep) && _model.CanAttack();
        }

        public void Attack(IUnitController target)
        {
            if (!CanAttack()) return;

            ChangeState(new AttackState(this, target));
        }

        public void Block(Attack attack)
        {
            if (state != UnitStateEnum.Idle) return;
            
            ChangeState(new BlockState(this, attack));
        }

        public void Evade(Attack attack)
        {
            if (state != UnitStateEnum.Idle) return;
            
            ChangeState(new EvadeState(this, attack));
        }

        public void NotifyOfIncomingAttack(Attack attack) => onGetAttacked?.Invoke(attack);

        public virtual AttackOutcome GetDamage(Attack attack)
        {
            AttackOutcome result;
            switch (state)
            {
                case UnitStateEnum.BlockPrep:
                    result = _model.TryBlockDamage(attack);
                    if (result.Result == AttackResult.Blocked)
                        _worldView.PlayBlocked();
                    break;
                case UnitStateEnum.Evading:
                    result = _model.TryEvadeDamage(attack);
                    _worldView.PlayTakeDamage(result);
                    break;
                default:
                    result = _model.GetDamage(attack);
                    _worldView.PlayTakeDamage(result);
                    break;
            }
            foreach (var uiView in _uiViews)
            {
                uiView.PlayTakeDamage(result);
                uiView.ShowNotification($"{result.Result} : {result.HpChange:F1}", _worldView.GetPosition() + Vector3.up * 2.5f);
            }
            
            onTakeDamage?.Invoke(result);
            
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

        public void Follow(IUnitController target)
        {
            throw new System.NotImplementedException();
        }

        public void Patrol(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void Protect(IUnitController target)
        {
            throw new System.NotImplementedException();
        }

        public void Protect(Vector3 position, float radius)
        {
            throw new System.NotImplementedException();
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
