using System.Collections.Generic;
using System.Linq;
using System.Text;
using Units.Enums;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace Units.Classes
{
    public class CharacterModel : IUnitModel
    {
        public string name { get; private set; }

        private Dictionary<LimbType, Limb> _limbs;
        private UnitAttributes _attributes;
        private UnitStats _stats;
        private bool _isDead = false, _isUnconscious = false;

        public CharacterModel(string name, UnitAttributes attributes, IEnumerable<Limb> limbs)
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

        public Attack GetAttack()
        {
            return new Attack(this, Random.value * _stats.MeleeAttack, new[] 
                { 
                    new Damage() 
                    { 
                        Amount = Random.Range(_stats.MeleeDmgMod * .5f, _stats.MeleeDmgMod * 1.1f),
                        Type = DamageType.Blunt 
                    } 
                },
                AttackType.Melee, Random.Range(0, _stats.MeleeAttack) >= _stats.MeleeAttack * .85f, false, null);
        }

        public float GetSwingTime()
        {
            return 1;
        }

        public AttackOutcome TakeDamage(Attack attack)
        {
            AttackOutcome outcome = new AttackOutcome()
            {
                Result = AttackResult.Full,
                HpChange = 0
            };

            if (CanBlockAttack(attack))
            {
                outcome.Result = AttackResult.Blocked;
                return outcome;
            }

            if (CanEvadeAttack(attack))
            {
                outcome.Result = AttackResult.Evaded;
                return outcome;
            }

            LimbType targetLimb;
            if (attack.LimbTarget.HasValue)
                targetLimb = attack.LimbTarget.Value;
            else
                targetLimb = (LimbType)Random.Range(0, 9);

            foreach (var damage in attack.Damage)
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

        public bool CanBlockAttack(Attack attack)
        {
            if (attack.IsSneak)
                return false;
            if (attack.IsCritical)
                return attack.AttackMod <= Random.value * _stats.MeleeDefence * .5f;
            return attack.AttackMod <= Random.value * _stats.MeleeDefence;
        }

        public bool CanEvadeAttack(Attack attack)
        {
            if (attack.IsSneak)
                return false;
            if (attack.IsCritical)
                return attack.AttackMod <= Random.value * _stats.MeleeEvade * .5f;
            return attack.AttackMod <= Random.value * _stats.MeleeEvade;
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