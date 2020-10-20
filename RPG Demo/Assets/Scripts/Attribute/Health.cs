using UnityEngine;
using RPG.Saving;
using System;
using RPG.Core;
using RPG.Stats;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {

        
        [SerializeField] int percentageLevelUp = 50;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;
        private Animator _anim;
        private bool _isDead;
        ActionScheduler _scheduler;
        CapsuleCollider _collider;
        BaseStats _baseStats;
        LazyValue<float> _health;

        float _heightAfterDead = 0.6f;
        float _yAxisAfterDead = 0.2f;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
            _collider = GetComponent<CapsuleCollider>();
            _baseStats = GetComponent<BaseStats>();
            // Impossibilita o erro de incializar os pontos de vida depois de restaura-los
            //if (_health < 0)
            //{
            //    _health = GetComponent<BaseStats>().GetStat(Stat.Health);
            //}
            _health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            _health.ForceInit();
        }

        public float GetHealth()
        {
            return _health.value;
        }

        public float GetMaxHealthPoints()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
            //print(gameObject.name + " took damage: " + damage);
            _health.value = Mathf.Max(_health.value - damage, 0);
            takeDamage.Invoke(damage);
            if(_health.value <= 0)
            {
                AwardExperience(instigator);
                Die();
            }
        }

        public void Heal(float pointsToHeal)
        {
            _health.value = Mathf.Min(_health.value + pointsToHeal, GetMaxHealthPoints());
        }

        public object GetStates()
        {
            return _health.value;
        }

        public void RestoreState(object state)
        {
            _health.value = (float)state;
            if (_health.value <= 0)
            {
                Die();
            }
        }

        public float GetPercentage()
        {
            return 100 * (GetHealthFraction());
        }

        public float GetHealthFraction()
        {
            return _health.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            _baseStats.onLevelUp += levelUpHealth;
        }

        private void OnDisable()
        {
            _baseStats.onLevelUp -= levelUpHealth;
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
                onDie.Invoke();
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

        private void levelUpHealth()
        {
            float maxHealth = _baseStats.GetStat(Stat.Health);
            float percentage = (maxHealth * percentageLevelUp) / 100;
            _health.value = Mathf.Min(percentage + _health.value, maxHealth);
        }
    }
}
