using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Units.Classes
{
    public class UnitsUIPopups : Singleton<UnitsUIPopups>
    {
        [SerializeField] private UIPopup _popupPrefab;

        private Queue<UIPopup> _pool;
        private List<UIPopup> _inUseList;

        private void Start()
        {
            _pool = new Queue<UIPopup>();
            _inUseList = new List<UIPopup>();
        }

        public void ShowPopup(Transform worldTransform, string message)
        {
            UIPopup popup;
            if (_pool.Count > 0)
                popup = _pool.Dequeue();
            else
                popup = CreatePopup();
            popup.gameObject.SetActive(true);
            popup.Show(worldTransform, message, 1);
            _inUseList.Add(popup);
        }

        public void ShowPopup(Vector3 worldPosition, string message)
        {
            UIPopup popup;
            if (_pool.Count > 0)
                popup = _pool.Dequeue();
            else
                popup = CreatePopup();
            popup.gameObject.SetActive(true);
            popup.Show(worldPosition, message, 1);
        }

        private UIPopup CreatePopup()
        {
            UIPopup popup = Instantiate(_popupPrefab, transform);
            popup.Initialize(() => PutPopupBack(popup));
            return popup;
        }

        private void PutPopupBack(UIPopup popup)
        {
            _inUseList.Remove(popup);
            popup.gameObject.SetActive(false);
            _pool.Enqueue(popup);
        }
    }
}
