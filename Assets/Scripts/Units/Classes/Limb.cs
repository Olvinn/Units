using Units.Enums;
using Units.Interfaces;
using UnityEngine;

namespace Units.Classes
{

    public class Limb : ILimb
    {
        public bool isDismemberable { get; }

        public bool isVitallyNecessary { get; }

        public LimbType type { get; }

        public Health health { get; }

        public Limb(float hp, bool isDismemberable, bool isVitallyNecessary, LimbType type)
        {
            health = new Health(hp);
            this.isDismemberable = isDismemberable;
            this.isVitallyNecessary = isVitallyNecessary;
            this.type = type;
        }

        public Limb(float hp, float maxHp, bool isDismemberable, bool isVitallyNecessary, LimbType type)
        {
            health = new Health(maxHp, hp);
            this.isDismemberable = isDismemberable;
            this.isVitallyNecessary = isVitallyNecessary;
            this.type = type;
        }

        public float GetStrength() => Mathf.Max(health.GetPercentage(), 0);
    }
}