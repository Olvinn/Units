using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class UIPopups : Singleton<UIPopups>
    {
        [SerializeField] private UIPopup _popupPrefab;

        private Queue<UIPopup> _pool;
        private List<UIPopup> _inUseList;

        private void Start()
        {
            _pool = new Queue<UIPopup>();
            _inUseList = new List<UIPopup>();
        }

        public void ShowPopup(Transform worldTransform, string message, float duration = 1)
        {
            UIPopup popup;
            if (_pool.Count > 0)
                popup = _pool.Dequeue();
            else
                popup = CreatePopup();
            popup.gameObject.SetActive(true);
            popup.Show(worldTransform, message, duration);
            _inUseList.Add(popup);
        }

        public void ShowPopup(Vector3 worldPosition, string message, float duration = 1)
        {
            UIPopup popup;
            if (_pool.Count > 0)
                popup = _pool.Dequeue();
            else
                popup = CreatePopup();
            popup.gameObject.SetActive(true);
            popup.Show(worldPosition, message, duration);
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
