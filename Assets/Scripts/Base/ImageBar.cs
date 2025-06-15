using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base
{
    public class ImageBar : MonoBehaviour
    {
        [SerializeField] private Image _bar, _bg;
        [SerializeField] private TMP_Text _label;

        public void UpdateUI(string name, float percentage)
        {
            _label.text = name;
            _bar.fillAmount = percentage;
        }
        public void UpdateUI(string name, float percentage, Color front, Color back)
        {
            _label.text = name;
            _bar.fillAmount = percentage;
            _bar.color = front;
            _bg.color = back;
        }
    }
}