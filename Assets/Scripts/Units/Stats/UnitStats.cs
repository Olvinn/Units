using System.Text;
using UnityEngine;

namespace Units.Stats
{
    public class UnitStats
    {
        public float MeleeDmgMod;
        public float MeleeAttack;
        public float MeleeDefence;
        public float MeleeEvade;
        public float SwingTime; //Time before swing actually do damage
        public float BlockTime; //Time before actually block damage
        public float StaggerDefence;
        public float AttackDistance;
        public float Speed;

        public UnitStats(UnitAttributes attributes)
        {
            MeleeDmgMod = attributes.Strength * 2 + attributes.Toughness;
            MeleeAttack = attributes.Agility + attributes.Perception + attributes.Strength * 2;
            MeleeDefence = attributes.Agility * 2 + attributes.Perception + attributes.Toughness;
            MeleeEvade = attributes.Perception + attributes.Agility * 2;
            SwingTime = 1 / Mathf.Log(attributes.Agility);
            BlockTime = 1 / Mathf.Log(attributes.Perception + attributes.Agility);
            StaggerDefence = attributes.Toughness + attributes.Will;
            AttackDistance = 1.5f;
            Speed = attributes.Strength * .5f + attributes.Agility;
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