using System.Text;
using UnityEngine;

namespace Units.Classes
{
    public class UnitStats
    {
        public float MeleeDmgMod;
        public float MeleeAttack;
        public float MeleeDefence;
        public float MeleeEvade;

        public UnitStats(UnitAttributes attributes)
        {
            MeleeDmgMod = (10 + attributes.Strength * 2 + attributes.Toughness) * .02f;
            MeleeAttack = Mathf.Sqrt(attributes.Agility + attributes.Perception + attributes.Strength * 2);
            MeleeDefence = Mathf.Sqrt(attributes.Agility * 2 + attributes.Perception + attributes.Strength);
            MeleeEvade = Mathf.Sqrt(attributes.Perception + attributes.Wisdom + attributes.Agility * 2);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Melee damage modifier: {MeleeDmgMod:F1}");
            sb.AppendLine($"Melee attack: {MeleeAttack:F1}");
            sb.AppendLine($"Melee defence: {MeleeDefence:F1}");
            sb.Append($"Melee evade: {MeleeEvade:F1}");
            return sb.ToString();
        }
    }
}