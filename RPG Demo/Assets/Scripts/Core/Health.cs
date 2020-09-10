using UnityEngine;
using System.Collections;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {

        [SerializeField] float _health = 100f;
        private Animator _anim;
        private bool _isDead;
        ActionScheduler _scheduler;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void TakeDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0);
            print(_health);
            if(_health == 0)
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
            }

        }
    }
}
