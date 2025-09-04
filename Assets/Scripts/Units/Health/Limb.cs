using UnityEngine;

namespace Units.Health
{

    public class Limb : ILimb
    {
        public bool isDismemberable { get; }

        public bool isVitallyNecessary { get; }

        public LimbType type { get; }

        public Units.Health.Health health { get; }

        public Limb(float hp, bool isDismemberable, bool isVitallyNecessary, LimbType type)
        {
            health = new Units.Health.Health(hp);
            this.isDismemberable = isDismemberable;
            this.isVitallyNecessary = isVitallyNecessary;
            this.type = type;
        }

        public Limb(float hp, float maxHp, bool isDismemberable, bool isVitallyNecessary, LimbType type)
        {
            health = new Units.Health.Health(maxHp, hp);
            this.isDismemberable = isDismemberable;
            this.isVitallyNecessary = isVitallyNecessary;
            this.type = type;
        }

        public float GetStrength() => Mathf.Max(health.GetPercentage(), 0);
    }
}