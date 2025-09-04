namespace Units.Health
{
    public interface ILimb
    {
        bool isDismemberable { get; }
    
        bool isVitallyNecessary { get; }
    
        LimbType type { get; }
    
        Health health { get; }
    
        /// <summary>
        /// How this limb is capable of doing things
        /// </summary>
        /// <returns>Percentage</returns>
        float GetStrength();
    }
}
