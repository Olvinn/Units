using UnityEngine;
using TMPro;
using Units.Classes;
using Units.Interfaces;
using UnityEngine.UI;

namespace Demos.FreeSpace.Scripts
{
    public class UnitViewPanel : MonoBehaviour, IUnitUIView
    {
        [SerializeField] private TextMeshProUGUI _name, _stats;
        [SerializeField] private Image _hpBar;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void ShowNotification(string message, Vector3 unitWorldPos)
        {
            UnitsUIPopups.Instance.ShowPopup(unitWorldPos, message, .85f);
        }

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
