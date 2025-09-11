using Base;
using TMPro;
using Units.Health;
using Units.Views;
using UnityEngine;

namespace Game.UI
{
    public class UnitViewPanel : MonoBehaviour, IUnitUIView
    {
        [SerializeField] private LimbsUIRepresentation _limbs;
        [SerializeField] private TMP_Text _statusTitle, _status;
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
            _statsTitle.text = Localization.UIStatusPanelName;
            _attributesTitle.text = Localization.UIAttributesPanelName;
            _statsTitle.text = Localization.UIStatsPanelName;
            _status.text = state.Status;
            _attributes.text = state.Attributes;
            _stats.text = state.Stats;
            _limbs.UpdateUI(state.LimbState);
        }

        public void PlayTakeDamage(AttackOutcome outcome)
        {
            
        }
    }
}
