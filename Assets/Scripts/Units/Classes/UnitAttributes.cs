using System.Text;

namespace Units.Classes
{
    public class UnitAttributes
    {
        public int Strength;
        public int Agility;
        public int Intelligent;
        public int Wisdom;
        public int Toughness;
        public int Will;
        public int Perception;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Strength: {Strength}");
            sb.AppendLine($"Agility: {Agility}");
            sb.AppendLine($"Intelligent: {Intelligent}");
            sb.AppendLine($"Toughness: {Toughness}");
            sb.AppendLine($"Perception: {Perception}");
            sb.AppendLine($"Wisdom: {Wisdom}");
            sb.Append($"Will: {Will}");
            return sb.ToString();
        }
    }
}