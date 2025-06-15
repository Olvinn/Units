using Base;
using TMPro;
using Units.Interfaces;
using Units.Structures;
using UnityEngine;

namespace FreeSpace.Scripts
{
    public class UnitViewPanel : MonoBehaviour, IUnitUIView
    {
        [SerializeField] private LimbsUIRepresentation _limbs;
        [SerializeField] private TMP_Text _attributesTitle, _attributes;
        [SerializeField] private TMP_Text _statsTitle, _stats;
        [SerializeField] private TextMeshProUGUI _name;

        public void ShowNotification(string message, Vector3 unitWorldPos)
        {
            UIPopups.Instance.ShowPopup(unitWorldPos, message, .85f);
        }

        public void UpdateView(UnitStateContainer state)
        {
            _name.text = name;
            _attributesTitle.text = Localization.UIAttributesPanelName;
            _statsTitle.text = Localization.UIStatsPanelName;
            _attributes.text = state.Attributes;
            _stats.text = state.Stats;
            _limbs.UpdateUI(state.LimbState);
        }

        public void PlayTakeDamage(AttackOutcome outcome)
        {
            
        }
    }
}
