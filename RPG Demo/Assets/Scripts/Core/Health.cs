using UnityEngine;
using System.Collections;
using RPG.Saving;
using System;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {

        [SerializeField] float _health = 100f;
        private Animator _anim;
        private bool _isDead;
        ActionScheduler _scheduler;
        CapsuleCollider _collider;

        float _heightAfterDead = 0.6f;
        float _yAxisAfterDead = 0.2f;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
            _collider = GetComponent<CapsuleCollider>();
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void TakeDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0);
            print(_health);
            if(_health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isDead == false)
            {
                _anim.SetTrigger("die");
                _isDead = true;
                _scheduler.CancelCurrentAction();
                AdjustCollider();
            }

        }

        private void AdjustCollider()
        {
            if(_collider != null)
            {
                _collider.height = _heightAfterDead;
                Vector3 centerCollider = _collider.center;
                centerCollider.y = _yAxisAfterDead;
                _collider.center = centerCollider;
            }
            
        }

        public object GetStates()
        {
            return _health;
        }

        public void RestoreState(object state)
        {
            _health = (float)state;
            if(_health <= 0)
            {
                Die();
            }
        }
    }
}
