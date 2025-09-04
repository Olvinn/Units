using System.Collections;
using Base;
using Units.Health;
using UnityEngine;

namespace Units.Views
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
            _animator.Play(AnimatorNames.AttackPreparation);
        }

        public void PlayBlockPrep(float speed)
        {
            _animator.SetFloat(AnimatorNames.Speed, speed);
            _animator.Play(AnimatorNames.BlockPreparation);
        }

        public void PlayBlocked()
        {
            _animator.Play(AnimatorNames.Block, 0, .01f);
        }

        public void PlayEvasion()
        {
            _animator.Play(AnimatorNames.Evade, 0, .01f);
        }

        public void PlayAttack()
        {
            _animator.Play(AnimatorNames.Attack);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void PlayTakeDamage(AttackOutcome outcome)
        {
            if (outcome.HpChange == 0) return;
            _animator.Play(AnimatorNames.Idle);
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
