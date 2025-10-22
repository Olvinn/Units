using System.Collections.Generic;
using System.Linq;
using System.Text;
using Units.Health;
using Units.Stats;
using Units.Views;
using UnityEngine;

namespace Units.Models
{
    public class UnitModel : IUnitModel
    {
        public string name { get; private set; }

        private Dictionary<LimbType, Limb> _limbs;
        private UnitAttributes _attributes;
        private UnitStats _stats;
        private bool _isDead = false, _isUnconscious = false;

        private bool _isDirty = true;
        private UnitStateContainer _stateContainer;

        public UnitModel(string name, UnitAttributes attributes, IEnumerable<Limb> limbs)
        {
            this.name = name;
            _attributes = attributes;
            _stats = new UnitStats(_attributes);
            _limbs = new();
            foreach (var limb in limbs)
                _limbs.Add(limb.type, limb);
        }

        public UnitStats GetStats() => _stats;

        public AttackData GetAttack()
        {
            var meleeAttack = _stats.MeleeAttack;
            return new AttackData(null, 
                meleeAttack, 
                new[] 
                { 
                    new DamageData() 
                    { 
                        Amount = Random.Range(0.75f, 1.25f) * _stats.MeleeDmgMod,
                        Type = DamageType.Blunt 
                    } 
                },
                AttackType.Melee, 
                meleeAttack > Random.Range(0.75f, 1.05f) * _stats.MeleeAttack,
                false, 
                null, 
                Time.time + GetSwingTime()
                );
        }

        public float GetSwingTime()
        {
            return _stats.SwingTime;
        }

        public float GetTimeToBlock()
        {
            return _stats.BlockTime;
        }

        public float GetStaggerTime(AttackOutcome attack)
        {
            switch (attack.TargetLimb)
            {
                case LimbType.Head:
                    return -attack.HpChange / _stats.StaggerDefence * 2;
                case LimbType.LeftArm:
                case LimbType.RightArm:
                case LimbType.LeftLeg:
                case LimbType.RightLeg:
                    return -attack.HpChange / _stats.StaggerDefence * .5f;
                default:
                    return -attack.HpChange / _stats.StaggerDefence;
            }
        }

        public UnitStateContainer GetStateContainer()
        {
            if (_isDirty)
            {
                Dictionary<LimbType, float> limbState = new Dictionary<LimbType, float>();
                foreach (var limbType in _limbs.Keys)
                {
                    limbState.Add(limbType, _limbs[limbType].health.GetPercentage());
                }

                _stateContainer = new UnitStateContainer()
                {
                    LimbState = limbState,
                    UnitName = name,
                    Attributes = _attributes.ToString(),
                    Stats = _stats.ToString()
                };
            }

            return _stateContainer;
        }

        public AttackOutcome TryBlockDamage(AttackData attackData)
        {
            if (CanBlockAttack(attackData))
            {
                AttackOutcome outcome = new AttackOutcome()
                {
                    ResultType = AttackResultType.Blocked,
                    HpChange = 0,
                    TargetLimb = attackData.TargetLimb
                };
                return outcome;
            }
            
            return GetDamage(attackData);
        }

        public AttackOutcome TryEvadeDamage(AttackData attackData)
        {
            if (CanEvadeAttack(attackData))
            {
                AttackOutcome outcome = new AttackOutcome()
                {
                    ResultType = AttackResultType.Evaded,
                    HpChange = 0,
                    TargetLimb = attackData.TargetLimb
                };
                return outcome;
            }

            return GetDamage(attackData);
        }

        public AttackOutcome GetDamage(AttackData attackData)
        {
            AttackOutcome outcome = new AttackOutcome()
            {
                ResultType = AttackResultType.Full,
                HpChange = 0,
                TargetLimb = attackData.TargetLimb
            };

            LimbType targetLimb;
            if (attackData.TargetLimb.HasValue)
                targetLimb = attackData.TargetLimb.Value;
            else
                targetLimb = (LimbType)Random.Range(0, LimbsStaticData.LimbsCount());

            foreach (var damage in attackData.Damage)
            {
                if (_limbs.ContainsKey(targetLimb))
                {
                    _limbs[targetLimb].health.TakeDamage(damage.Amount);
                    outcome.HpChange -= damage.Amount;
                }
                else
                {
                    int limbInd = Random.Range(0, _limbs.Count);
                    _limbs.Values.ToArray()[limbInd].health.TakeDamage(damage.Amount);
                    outcome.HpChange -= damage.Amount;
                }
            }

            _isDirty = true;

            return outcome;
        }

        private bool CanBlockAttack(AttackData attackData)
        {
            if (attackData.IsSneak)
                return false;
            if (attackData.IsCritical)
                return attackData.AttackMod <= Random.Range(0.75f, 1.1f) * _stats.MeleeDefence * .5f;
            return attackData.AttackMod <= Random.Range(0.75f, 1.1f) * _stats.MeleeDefence;
        }

        private bool CanEvadeAttack(AttackData attackData)
        {
            if (attackData.IsSneak)
                return false;
            if (attackData.IsCritical)
                return attackData.AttackMod <= Random.Range(0.75f, 1.1f) * _stats.MeleeEvade * .5f;
            return attackData.AttackMod <= Random.Range(0.75f, 1.1f) * _stats.MeleeEvade;
        }
    }
}