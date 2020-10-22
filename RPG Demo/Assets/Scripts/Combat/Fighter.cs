using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] Transform _rightHandTransform;
        [SerializeField] Transform _leftHandTransform;
        [SerializeField] WeaponConfig defaultWeapon;
        [SerializeField] float _breath = 1f;
        Health target;
        private Mover _mover;
        ActionScheduler _scheduler;
        private Animator _anim;
        //[SerializeField] float _damage = 25f;
        //[SerializeField] private float _distanceRange = 2f;
        float _currentTime = 0;
        float _attackTime = 0;
        private BaseStats _baseStats;
        //[SerializeField] string defaultWeaponName = "Unarmed";
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;


        private void Awake()
        {
            _mover = GetComponent<Mover>();
            if (_mover == null)
            {
                Debug.LogError("Mover is Null");
            }
            _scheduler = GetComponent<ActionScheduler>();
            if (_scheduler == null)
            {
                Debug.LogError("ActionScheduler is Null");
            }

            _anim = GetComponent<Animator>();
            if (_anim == null)
            {
                Debug.LogError("Animator is Null");
            }

            _baseStats = GetComponent<BaseStats>();
            if (_baseStats == null)
            {
                Debug.LogError("BaseStats is Null");
            }

            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(GetDeafultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
           
        }

        private void Update()
        {
            _currentTime = Time.time;
            if (target != null)
            {
                if (target.IsDead()) return;

                if (!GetInRange(target.transform.position))
                {
                    _mover.MoveToDestination(target.transform.position);
                }
                else
                {
                    _mover.Cancel();
                    AttackBeahvior();  
                }
            }
            
        }

        private void AttackBeahvior()
        {
            if (_currentTime > _attackTime)
            {
                transform.LookAt(target.transform);
                _anim.ResetTrigger("stopAttack");
                //A animação irá chamar a função Hit()
                _anim.SetTrigger("attack");
                _attackTime = _currentTime + _breath;
            }
                
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (!_mover.CanMoveTo(combatTarget.transform.position) && !GetInRange(combatTarget.transform.position)) return false;
            Health targetHealth = combatTarget.GetComponent<Health>();
            return combatTarget != null && !targetHealth.IsDead();
        }

        public Health GetTarget()
        {
            return target;
        }

        // Função chamada pelo animator
        void Hit()
        {
            float damage = _baseStats.GetStat(Stat.Damage);
            if(target != null)
            {
                if(currentWeapon.value != null)
                {
                    currentWeapon.value.OnHit();
                }
                // Se a arma tiver projetil, ira lancar o projetil. Senao ele vai atacar
                if (currentWeaponConfig.HasProjectile())
                {
                    currentWeaponConfig.LauchProjectile(_rightHandTransform, _leftHandTransform, target, GetComponent<Collider>(), damage);
                }
                else
                {
                    target.TakeDamage(damage, gameObject);
                }
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetInRange(Vector3 target)
        {
            return Vector3.Distance(transform.position, target) < currentWeaponConfig.GetWeaponRange();
        }

        public void Cancel()
        {
            _anim.SetTrigger("stopAttack");
            target = null;
            _mover.Cancel();
        }

        public void Attack(GameObject combatTarget)
        {
            _scheduler.StartAction(this);
            target = combatTarget.transform.GetComponent<Health>();
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon GetDeafultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            if (weapon != null)
            {
                return weapon.SpawnWeapon(_anim, _rightHandTransform, _leftHandTransform);
            }
            else
            {
                return null;
            }
        }

        public object GetStates()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string restoredWeaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(restoredWeaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentagteBonus();
            }
        }
    }
}
