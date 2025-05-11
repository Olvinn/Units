using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace Units.Classes
{
    public class UnitWorldView : MonoBehaviour, IUnitWorldView
    {
        public void PlayAttack()
        {
            
        }

        public void PlayTakeDamage(Attack damage, AttackOutcome outcome)
        {
            
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}
