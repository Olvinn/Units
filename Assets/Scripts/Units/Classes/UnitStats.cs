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
        public float SwingTime; //Time before swing actually do damage
        public float BlockTime; //Time before actually block damage

        public UnitStats(UnitAttributes attributes)
        {
            MeleeDmgMod = (10 + attributes.Strength * 2 + attributes.Toughness) * .2f;
            MeleeAttack = attributes.Agility + attributes.Perception + attributes.Strength * 2;
            MeleeDefence = attributes.Agility * 2 + attributes.Perception + attributes.Strength;
            MeleeEvade = attributes.Perception + attributes.Wisdom + attributes.Agility * 2;
            SwingTime = (float)5 / attributes.Agility;
            BlockTime = (float)10 / attributes.Agility;
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