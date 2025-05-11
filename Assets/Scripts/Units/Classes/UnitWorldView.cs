using System;
using System.Collections;
using Units.Interfaces;
using UnityEngine;

namespace Units.Classes
{
    public class UnitWorldView : MonoBehaviour, IUnitWorldView
    {
        [SerializeField] private Renderer _renderer;
        private Coroutine _takingDamageCoroutine;
        private Color _unitColor;

        private void Start()
        {
            _unitColor = _renderer.material.color;
        }

        public void PlayAttack()
        {
            
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void PlayTakeDamage(float damage, float currentHPPercent)
        {
            if (_takingDamageCoroutine != null)
                StopCoroutine(_takingDamageCoroutine);
            StartCoroutine(TakingDamageCoroutine(1));
        }

        IEnumerator TakingDamageCoroutine(float time)
        {
            Material mat = _renderer.material;
            while (time > 0)
            {
                if (mat.color == Color.white)
                    mat.color = _unitColor;
                else
                    mat.color = Color.white;
                time -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            mat.color = _unitColor;
        }
    }
}
