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

        public void Play(Cue cue)
        {
            switch (cue)
            {
                case Cue.Idle:
                    _animator.Play(AnimatorNames.Idle);
                    break;
                case Cue.AttackPreparation:
                    _animator.Play(AnimatorNames.AttackPreparation);
                    break;
                case Cue.BlockPreparation:
                    _animator.Play(AnimatorNames.BlockPreparation);
                    break;
                case Cue.Block:
                    _animator.Play(AnimatorNames.Block);
                    break;
                case  Cue.Evade:
                    _animator.Play(AnimatorNames.Evade);
                    break;
                case Cue.Attack:
                    _animator.Play(AnimatorNames.Attack);
                    break;
            }
            _animator.SetFloat(AnimatorNames.Speed, 1);
        }

        public void Play(Cue cue, float speed)
        {
            Play(cue);
            _animator.SetFloat(AnimatorNames.Speed, speed);
        }

        public Transform GetTransform()
        {
            return transform;
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
