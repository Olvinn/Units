using System.Collections.Generic;
using Units.Enums;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace Units.Classes
{
    public abstract class BaseUnitController : IUnitController
    {
        private IUnitModel _model;
        private IUnitMovement _movement;
        private IUnitSense _sense;
        private IUnitWorldView _worldView;
        private IUnitUIView[] _uiViews;

        private UnitStatus _status;
        
        private float _swingTimer;
        private Attack _attack;
        private IEnumerable<IUnitController> _targets;
    
        protected BaseUnitController(IUnitModel model, IUnitMovement movement, IUnitSense sense, IUnitWorldView worldView, IUnitUIView[] uiViews)
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

            if (_swingTimer > 0)
            {
                _swingTimer -= dt;
                if (_swingTimer <= 0)
                {
                    _worldView.PlayAttack();
                    if (_targets != null)
                        foreach (var target in _targets)
                        {
                            var outcome = target.TakeDamage(_attack);
                        }

                    _status = UnitStatus.Idle;
                }
            }

            foreach (var uiView in _uiViews)
                uiView.UpdateView(_model.name, _model.ToString());
        }

        public IUnitModel GetModel() => _model;
        
        public Vector3 GetPosition() => _worldView.GetPosition();
        
        public bool CanAttack()
        {
            return _status == UnitStatus.Idle && _model.CanAttack();
        }

        public void Attack(IUnitController target)
        {
            if (!CanAttack()) return;
            
            _attack = _model.GetAttack();
            _targets = WorldUnits.GetTargetsAffectedByAttack(this, target, _attack);

            _swingTimer = _model.GetSwingTime();
            _status = UnitStatus.Attacking;
            _worldView.PlayAttackPrep();

            foreach (var t in _targets)
                t.NotifyOfIncomingAttack(_attack);

            _status = UnitStatus.Attacking;
        }

        public void NotifyOfIncomingAttack(Attack attack)
        {
            _status = UnitStatus.Defending;
        }

        public virtual AttackOutcome TakeDamage(Attack attack)
        {
            var result = _model.TakeDamage(attack);
            _worldView.PlayTakeDamage(result.HpChange, _model.GetFullHPPercent());
            foreach (var uiView in _uiViews)
            {
                uiView.PlayTakeDamage(result.HpChange, _model.GetFullHPPercent());
                uiView.ShowNotification($"{result.Result} : {result.HpChange:F1}", _worldView.GetPosition() + Vector3.up * 2.5f);
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
