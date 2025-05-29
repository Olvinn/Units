using System;
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

        public UnitState state { get; private set; }
        
        private IUnitModel _model;
        private IUnitMovement _movement;
        private IUnitSense _sense;
        private IUnitWorldView _worldView;
        private IUnitUIView[] _uiViews;
        
        private float _swingTimer;
        private Attack _attack;
        private IUnitController _target;
    
        public UnitController(IUnitModel model, IUnitMovement movement, IUnitSense sense, IUnitWorldView worldView, IUnitUIView[] uiViews)
        {
            _model = model;
            _movement = movement;
            _sense = sense;
            _worldView = worldView;
            _uiViews = uiViews;
        }

        public virtual void Update(float dt)
        {
            _movement?.Update(dt);
            _sense?.Update(dt);

            AttackUpdate(dt);

            foreach (var uiView in _uiViews)
                uiView.UpdateView(_model.name, _model.ToString());
        }

        private void AttackUpdate(float dt)
        {
            if (state != UnitState.Attacking)
            {
                _swingTimer = -1;
                return;
            }
            
            if (_swingTimer > 0)
            {
                _swingTimer -= dt;
                if (_swingTimer <= 0)
                {
                    _worldView.PlayAttack();
                    if (_target != null)
                    {
                        var outcome = _target.GetDamage(_attack);
                    }

                    state = UnitState.Idle;
                }
            }
        }
        public IUnitModel GetModel() => _model;
        
        public Vector3 GetPosition() => _worldView.GetPosition();
        
        public bool CanAttack()
        {
            return (state == UnitState.Idle || state == UnitState.Evading || state == UnitState.Blocking) && _model.CanAttack();
        }

        public void Attack(IUnitController target)
        {
            if (!CanAttack()) return;
            
            _attack = _model.GetAttack();
            _attack.Source = this;
            _target = target;

            _swingTimer = _attack.ApproxHitTime - Time.time;
            _worldView.PlayAttackPrep(1 / _model.GetSwingTime());

            _target.NotifyOfIncomingAttack(_attack);

            state = UnitState.Attacking;
        }

        public void Block(Attack attack)
        {
            if (state != UnitState.Idle) return;
            
            _worldView.PlayBlockPrep(1 / _model.GetTimeToBlock());
            
            state = UnitState.Blocking;
        }

        public void Evade(Attack attack)
        {
            if (state != UnitState.Idle) return;
            
            _worldView.PlayEvasion();

            state = UnitState.Evading;
        }

        public void NotifyOfIncomingAttack(Attack attack) => onGetAttacked?.Invoke(attack);

        public virtual AttackOutcome GetDamage(Attack attack)
        {
            AttackOutcome result;
            switch (state)
            {
                case UnitState.Blocking:
                    result = _model.TryBlockDamage(attack);
                    if (result.Result == AttackResult.Blocked)
                        _worldView.PlayBlocked();
                    break;
                case UnitState.Evading:
                    result = _model.TryEvadeDamage(attack);
                    _worldView.PlayTakeDamage(result.HpChange, _model.GetFullHPPercent());
                    break;
                default:
                    result = _model.GetDamage(attack);
                    _worldView.PlayTakeDamage(result.HpChange, _model.GetFullHPPercent());
                    break;
            }
            foreach (var uiView in _uiViews)
            {
                uiView.PlayTakeDamage(result.HpChange, _model.GetFullHPPercent());
                uiView.ShowNotification($"{result.Result} : {result.HpChange:F1}", _worldView.GetPosition() + Vector3.up * 2.5f);
            }
            if (result.Result == AttackResult.Full)
                _swingTimer = -1;
            state = UnitState.Idle;
            
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
    }
}
