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
        private HashSet<IUnitModel> _knownEnemies;
        private bool _isDead = false, _isUnconscious = false;

        public CharacterModel(string name, UnitAttributes attributes, IEnumerable<Limb> limbs)
        {
            this.name = name;
            _attributes = attributes;
            _stats = new UnitStats(_attributes);
            _limbs = new();
            foreach (var limb in limbs)
                _limbs.Add(limb.type, limb);
            _knownEnemies = new();
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
            return new Attack(this, 1, new[] { new Damage() { Amount = 50, Type = DamageType.Blunt } },
                AttackType.Melee, false, null);
        }

        public float GetSwingTime()
        {
            return 1;
        }

        public AttackOutcome TakeDamage(Attack attack)
        {
            bool isSneakAttack = !_knownEnemies.Contains(attack.Source);

            AttackOutcome outcome = new AttackOutcome()
            {
                Result = AttackResult.Full,
                HpChange = 0
            };

            if (IsBlocked(attack, Random.value, isSneakAttack))
                outcome.Result = AttackResult.Blocked;

            if (IsEvaded(attack, Random.value, isSneakAttack))
                outcome.Result = AttackResult.Evaded;

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

        private bool IsBlocked(Attack attack, float random, bool isSneak)
        {
            if (isSneak)
                return false;
            if (attack.IsCritical)
                return attack.AttackMod <= random * _stats.MeleeDefence * .5f;
            return attack.AttackMod <= random * _stats.MeleeDefence;
        }

        private bool IsEvaded(Attack attack, float random, bool isSneak)
        {
            if (isSneak)
                return false;
            if (attack.IsCritical)
                return attack.AttackMod <= random * _stats.MeleeEvade * .5f;
            return attack.AttackMod <= random * _stats.MeleeEvade;
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