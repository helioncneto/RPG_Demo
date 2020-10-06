using UnityEngine;
using System.Collections;
using RPG.Saving;
using System;
using RPG.Core;
using RPG.Stats;

namespace RPG.Resources
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
            _health = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            //_health = GetComponent<BaseStats>().GetHealth();
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
            _health = Mathf.Max(_health - damage, 0);
            //print(_health);
            if(_health <= 0)
            {
                AwardExperience(instigator);
                Die();
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience != null)
            {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
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

        public float GetPercentage()
        {
            return 100 * (_health / GetComponent<BaseStats>().GetStat(Stat.Health));
        }
    }
}
