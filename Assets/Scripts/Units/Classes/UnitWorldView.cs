using System.Collections;
using Units.Interfaces;
using UnityEngine;

namespace Units.Classes
{
    public class UnitWorldView : MonoBehaviour, IUnitWorldView
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Animator _animator;
        
        private Coroutine _takingDamageCoroutine;
        private Color _unitColor;

        private void Start()
        {
            _unitColor = _renderer.material.color;
        }

        public void PlayAttackPrep(float speed)
        {
            _animator.SetFloat(AnimatorNames.Speed, speed);
            _animator.SetTrigger(AnimatorNames.AttackPreparation);
        }
        
        public void PlayAttack()
        {
            _animator.SetTrigger(AnimatorNames.Attack);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void PlayTakeDamage(float damage, float currentHPPercent)
        {
            if (damage == 0) return;
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
