using System.Collections.Generic;
using System.Linq;
using System.Text;
using Units.Controllers;
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

        public UnitModel(string name, UnitAttributes attributes, IEnumerable<Limb> limbs)
        {
            this.name = name;
            _attributes = attributes;
            _stats = new UnitStats(_attributes);
            _limbs = new();
            foreach (var limb in limbs)
                _limbs.Add(limb.type, limb);
        }

        public float GetFullHPPercent()
        {
            float full = 0, current = 0;
            foreach (var limb in _limbs.Values)
            {
                full += limb.health.GetMaxHP();
                current += limb.health.GetHP();
            }
            return current / full;
        }

        public UnitStats GetStats() => _stats;

        public bool CanAttack()
        {
            return _limbs[LimbType.RightArm].health.GetHP() > 0 && !_isDead && !_isUnconscious;
        }

        public AttackData GetAttack()
        {
            return new AttackData(null, 
                Random.value * _stats.MeleeAttack, 
                new[] 
                { 
                    new DamageData() 
                    { 
                        Amount = Random.Range(_stats.MeleeDmgMod * .5f, _stats.MeleeDmgMod * 1.1f),
                        Type = DamageType.Blunt 
                    } 
                },
                AttackType.Melee, 
                Random.Range(0, _stats.MeleeAttack) >= _stats.MeleeAttack * .85f,
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
            Dictionary<LimbType, float> limbState = new Dictionary<LimbType, float>();
            foreach (var limbType in _limbs.Keys)
            {
                limbState.Add(limbType, _limbs[limbType].health.GetPercentage());
            }
            return new UnitStateContainer()
            {
                LimbState = limbState,
                UnitName = name,
                Attributes = _attributes.ToString(),
                Stats = _stats.ToString()
            };
        }

        public AttackOutcome TryBlockDamage(AttackData attackData)
        {
            if (CanBlockAttack(attackData))
            {
                AttackOutcome outcome = new AttackOutcome()
                {
                    ResultType = AttackResultType.Full,
                    HpChange = 0,
                    TargetLimb = attackData.TargetLimb
                };
                outcome.ResultType = AttackResultType.Blocked;
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
                    ResultType = AttackResultType.Full,
                    HpChange = 0,
                    TargetLimb = attackData.TargetLimb
                };
                outcome.ResultType = AttackResultType.Evaded;
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
                targetLimb = (LimbType)Random.Range(0, 9);

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

            return outcome;
        }

        private bool CanBlockAttack(AttackData attackData)
        {
            if (attackData.IsSneak)
                return false;
            if (attackData.IsCritical)
                return attackData.AttackMod <= Random.value * _stats.MeleeDefence * .5f;
            return attackData.AttackMod <= Random.value * _stats.MeleeDefence;
        }

        private bool CanEvadeAttack(AttackData attackData)
        {
            if (attackData.IsSneak)
                return false;
            if (attackData.IsCritical)
                return attackData.AttackMod <= Random.value * _stats.MeleeEvade * .5f;
            return attackData.AttackMod <= Random.value * _stats.MeleeEvade;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine();
            sb.AppendLine($"- {name} -");
            sb.AppendLine("- Attributes -");
            sb.AppendLine(_attributes.ToString());
            sb.AppendLine("--- Stats ----");
            sb.AppendLine(_stats.ToString());
            sb.AppendLine("---- Limbs ---");
            foreach (var limb in _limbs.Values)
                sb.AppendLine($"{limb.type} : {limb.health.GetHP()}/{limb.health.GetMaxHP():F1}");
            sb.Append("--------------");
            return sb.ToString();
        }
    }
}