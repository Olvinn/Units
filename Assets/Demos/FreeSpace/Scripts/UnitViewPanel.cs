using UnityEngine;
using TMPro;
using Units.Interfaces;
using UnityEngine.UI;

namespace Demos.FreeSpace.Scripts
{
    public class UnitViewPanel : MonoBehaviour, IUnitUIView
    {
        [SerializeField] private TextMeshProUGUI _name, _stats;
        [SerializeField] private Image _hpBar;

        public void UpdateView(string name, string stats)
        {
            _name.text = name;
            UpdateView(stats);
        }
        
        public void UpdateView(string stats)
        {
            _stats.text = stats;
        }

        public void PlayTakeDamage(float damage, float currentHPPercent)
        {
            _hpBar.fillAmount = currentHPPercent;
        }
    }
}
