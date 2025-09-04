using Units.Health;
using Units.Stats;
using UnityEngine;

namespace Units.Models
{
    public static class UnitFactory
    {
        public static UnitModel CreateRandomUnit(int lvl, string name = "Dummy")
        {
            int freeAttributesPoints = GetFreeAttributesPoints(lvl);
            UnitAttributes attributes = CreateRandomAttributes(freeAttributesPoints);
            Limb[] limbs = CreateHumanoidLimbs(attributes.Toughness);
            return new UnitModel(name, attributes, limbs);
        }

        public static UnitModel CreateUniformUnit(int lvl, string name = "Dummy")
        {
            int freeAttributesPoints = GetFreeAttributesPoints(lvl);
            UnitAttributes attributes = CreateRandomAttributes(freeAttributesPoints);
            Limb[] limbs = CreateHumanoidLimbs(attributes.Toughness);
            return new UnitModel(name, attributes, limbs);
        }

        public static Limb[] CreateHumanoidLimbs(int toughness)
        {
            int mod = (int)(Mathf.Sqrt(toughness * 400));
            Limb[] res = new[]
            {
                new Limb(mod, true, true, LimbType.Head),
                new Limb(mod * .5f, false, true, LimbType.Neck),
                new Limb(mod * 1.1f, false, true, LimbType.Chest),
                new Limb(mod * .9f, false, true, LimbType.Stomach),
                new Limb(mod * .8f, true, false, LimbType.Groin),
                new Limb(mod, true, false, LimbType.LeftArm),
                new Limb(mod, true, false, LimbType.RightArm),
                new Limb(mod, true, false, LimbType.LeftLeg),
                new Limb(mod, true, false, LimbType.RightLeg),
            };
            return res;
        }

        public static int GetFreeAttributesPoints(int lvl)
        {
            return lvl * 2;
        }

        public static UnitAttributes GetUniformAttributes(int freePoints)
        {
            UnitAttributes result = new UnitAttributes();
            result.Agility = result.Intelligent = result.Perception = result.Strength = result.Will =
                result.Toughness = result.Wisdom = 1;
            for (int i = 0; i < freePoints; i++)
                UpRandomAttribute(result, (float)i / freePoints % 1f);
            return result;
        }

        public static UnitAttributes CreateRandomAttributes(int freePoints)
        {
            UnitAttributes result = new UnitAttributes();

            result.Agility = result.Intelligent = result.Perception = result.Strength = result.Will =
                result.Toughness = result.Wisdom = 1;

            for (int i = 0; i < freePoints; i++)
                UpRandomAttribute(result, Random.value);
            return result;
        }

        public static void UpRandomAttribute(UnitAttributes attributes, float random)
        {
            switch (random)
            {
                case < 1 / 7f:
                    attributes.Strength++;
                    return;
                case < 2 / 7f:
                    attributes.Agility++;
                    return;
                case < 3 / 7f:
                    attributes.Intelligent++;
                    return;
                case < 4 / 7f:
                    attributes.Toughness++;
                    return;
                case < 5 / 7f:
                    attributes.Perception++;
                    return;
                case < 6 / 7f:
                    attributes.Wisdom++;
                    return;
                default:
                    attributes.Will++;
                    return;
            }
        }
    }
}