using System.Collections;
using UnityEngine;

namespace Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Widget : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        private bool _isActive;

        private void Awake()
        {
            if (_group == null)
                _group = GetComponent<CanvasGroup>();
        }

        private void OnValidate()
        {
            if (_group == null)
                _group = GetComponent<CanvasGroup>();
        }

        public void Show(float speed = .25f)
        {
            if (_isActive) return;
            _isActive = true;
            StopAllCoroutines();
            StartCoroutine(ShowCoroutine(speed));
        }

        public void Hide(float speed = .25f)
        {
            if (!_isActive) return;
            _isActive = false;
            StopAllCoroutines();
            StartCoroutine(HideCoroutine(speed));
        }

        private IEnumerator ShowCoroutine(float speed)
        {
            var start = Time.time;
            while (Time.time < start + speed)
            {
                _group.alpha += 1 / speed * Time.deltaTime;
                yield return null;
            }
            _group.alpha = 1;
        }

        private IEnumerator HideCoroutine(float speed)
        {
            var start = Time.time;
            while (Time.time < start + speed)
            {
                _group.alpha -= 1 / speed * Time.deltaTime;
                yield return null;
            }
            _group.alpha = 0;
        }
    }
}
