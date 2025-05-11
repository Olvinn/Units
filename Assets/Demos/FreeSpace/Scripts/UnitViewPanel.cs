using UnityEngine;
using TMPro;
using Units.Interfaces;
using Units.Structures;

namespace Demos.FreeSpace.Scripts
{
    public class UnitViewPanel : MonoBehaviour, IUnitUIView
    {
        [SerializeField] private TextMeshProUGUI _name, _stats;

        public void UpdateView(string name, string stats)
        {
            _name.text = name;
            UpdateView(stats);
        }
        
        public void UpdateView(string stats)
        {
            _stats.text = stats;
        }

        public void PlayAttack()
        {
            throw new System.NotImplementedException();
        }

        public void PlayTakeDamage(Attack damage, AttackOutcome outcome)
        {
            throw new System.NotImplementedException();
        }
    }
}
