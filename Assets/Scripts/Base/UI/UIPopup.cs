using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Base
{
    [RequireComponent(typeof(Widget))]
    public class UIPopup : MonoBehaviour, IPoolable
    {
        public Action OnDone { get; private set; }
        
        [SerializeField] private Widget _widget;
        [SerializeField] private TextMeshProUGUI _label;

        private Transform _targetTransform;
        private Vector3 _targetPosition;
        private Camera _camera;

        private void Start()
        {
            if (_widget == null)
                _widget = GetComponentInChildren<Widget>();
            if (_label == null)
                _label = GetComponentInChildren<TextMeshProUGUI>();
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_targetTransform)
                _targetPosition = _targetTransform.position;

            var screenPos = _camera.WorldToScreenPoint(_targetPosition);
            if (screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
                _widget.Hide(.1f);
            else
                _widget.Show(.1f);
            _widget.transform.position = screenPos;
        }

        private void OnValidate()
        {
            if (_widget == null)
                _widget = GetComponentInChildren<Widget>();
            if (_label == null)
                _label = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Initialize(Action onDone)
        {
            OnDone = onDone;
        }

        public void Show(Transform worldTransform, string message)
        {
            _targetTransform = worldTransform;
            _label.text = message;
            _widget.Show(0.1f);
            StopAllCoroutines();
        }
        
        public void Show(Transform worldTransform, string message, float duration)
        {
            _targetTransform = worldTransform;
            _label.text = message;
            _widget.Show(0.1f);
            StopAllCoroutines();
            StartCoroutine(Timer(duration, Hide));
        }

        public void Show(Vector3 worldPosition, string message)
        {
            _targetPosition = worldPosition;
            _label.text = message;
            _widget.Show(0.1f);
            StopAllCoroutines();
        }
        
        public void Show(Vector3 worldPosition, string message, float duration)
        {
            _targetPosition = worldPosition;
            _label.text = message;
            _widget.Show(0.1f);
            StopAllCoroutines();
            StartCoroutine(Timer(duration, Hide));
        }

        public void Hide()
        {
            _widget.Hide(0.1f);
            StopAllCoroutines();
            _targetTransform = null;
            OnDone?.Invoke();
        }

        private IEnumerator Timer(float duration, Action onEnd)
        {
            yield return new WaitForSeconds(duration);
            onEnd?.Invoke();
        }
    }
}
