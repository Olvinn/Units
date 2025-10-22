using System;
using Units.Controllers.ControllerStates;
using Units.Health;
using Units.Models;
using Units.Views;
using UnityEngine;

namespace Units.Controllers
{
    public class UnitController : IUnitController, IUnitStateMachine
    {
        public event Action<AttackData> onGetAttacked;
        public event Action<AttackOutcome> onTakeDamage;

        public UnitStateEnum state
        {
            get
            {
                if (_isStateOverwritten)
                    return _overwrittenState;
                return _currentJob.stateEnum;
            }
        }

        private bool _isStateOverwritten;
        private UnitStateEnum _overwrittenState;
        
        private IUnitModel _model;
        private IUnitMovement _movement;
        private IUnitWorldView _worldView;
        private IUnitUIView[] _uiViews;

        private UnitControllerState _currentJob;
    
        public UnitController(IUnitModel model, IUnitMovement movement, IUnitWorldView worldView, IUnitUIView[] uiViews)
        {
            _model = model;
            _movement = movement;
            _worldView = worldView;
            _uiViews = uiViews;
            _currentJob = new IdleState();
            _currentJob.Do();
        }

        public void Update(float dt)
        {
            _movement?.Update(dt);
            _currentJob.Update(dt);

            var data = _model.GetStateContainer();
            data.Status = state.ToString();

            foreach (var uiView in _uiViews)
                uiView.UpdateView(data);
        }

        public AttackData GetAttack()
        {
            var attack = _model.GetAttack();
            attack.Source = this;
            return attack;
        }

        public IUnitModel GetModel() => _model;
        public IUnitWorldView GetWorldView() => _worldView;
        public IUnitMovement GetMovement() => _movement;
        public Transform GetTransform() => _worldView.GetTransform();
        
        private bool CanAttack() => _currentJob.CanAttack();
        private bool CanBlock() => _currentJob.CanBlock();
        private bool CanEvade() => _currentJob.CanEvade();
        private bool CanMove() => _currentJob.CanMove();

        public void Attack(IUnitController target)
        {
            if (target == null) return;
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

        public void Move(IUnitController target)
        {
            if (!CanMove()) return;
                ChangeState(new MovingState(this, target));
        }

        public void Move(Vector3 destination)
        {
            if (!CanMove()) return;
                ChangeState(new MovingState(this, destination));
        }

        public void NotifyOfIncomingAttack(AttackData attackData) => onGetAttacked?.Invoke(attackData);

        public AttackOutcome TakeDamage(AttackData attackData)
        {
            var result =  _currentJob.TakeDamage(attackData);
            
            if (result.ResultType == AttackResultType.Full)
                ChangeState(new StaggeringState(this, result));
            
            foreach (var uiView in _uiViews)
            {
                uiView.PlayTakeDamage(result);
                uiView.ShowNotification($"{result.ResultType} : {result.HpChange:F1}", 
                    _worldView.GetTransform().position + Vector3.up * 2.5f);
            }
            
            onTakeDamage?.Invoke(result);
            return result;
        }

        public void Do(UnitControllerState job)
        {
            ChangeState(job);
        }

        private void ChangeState(UnitControllerState newState)
        {
            _currentJob.FinishSilent();
            _currentJob.Dispose();
            _currentJob.onDone -= Finishing;
            _currentJob = newState;
            _currentJob.onDone += Finishing;
            _currentJob.Do();
            _isStateOverwritten = false;
        }

        private void Finishing()
        {
            _currentJob.FinishSilent();
            _currentJob = new IdleState();
            _currentJob.Do();
            _isStateOverwritten = false;
        }
    }
}
