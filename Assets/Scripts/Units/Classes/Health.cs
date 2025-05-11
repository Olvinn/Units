using System;
using UnityEngine;

namespace Units.Classes
{
    public class Health
    {
        public event Action onHpReachZero;
        private float _hp, _maxHp;

        public Health(float maxHp)
        {
            _hp = _maxHp = maxHp;
        }

        public Health(float maxHp, float hp)
        {
            _hp = hp;
            _maxHp = maxHp;
        }

        public float GetHP() => _hp;

        public float GetMaxHP() => _maxHp;

        public float GetPercentage() => _hp / _maxHp;

        public float GetOverkill() => Mathf.Min(0, _hp);

        public void TakeDamage(float damage)
        {
            _hp -= damage;
            if (_hp <= 0)
                onHpReachZero?.Invoke();
        }

        public void TakeHealing(float heal)
        {
            _hp += heal;
            _hp = Mathf.Min(_maxHp, _hp);
        }

        public void SetHp(float hp)
        {
            _hp = hp;
            _hp = Mathf.Min(_maxHp, _hp);
        }
    }
}