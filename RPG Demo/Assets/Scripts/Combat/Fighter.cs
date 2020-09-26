using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        Health target;
        private Mover _mover;
        ActionScheduler _scheduler;
        private Animator _anim;
        [SerializeField] float _breath = 1f;
        //[SerializeField] float _damage = 25f;
        //[SerializeField] private float _distanceRange = 2f;
        float _currentTime = 0;
        float _attackTime = 0;
        [SerializeField] Transform _rightHandTransform;
        [SerializeField] Transform _leftHandTransform;
        [SerializeField] Weapon defaultWeapon;
        Weapon currentWeapon;

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

            
        }

        private void Start()
        {
            currentWeapon = defaultWeapon;
            EquipWeapon(currentWeapon);
        }

        private void Update()
        {
            _currentTime = Time.time;
            if (target != null)
            {
                if (target.IsDead()) return;

                if (!GetInRange())
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
            Health targetHealth = combatTarget.GetComponent<Health>();
            return combatTarget != null && !targetHealth.IsDead();
        }

        // Função chamada pelo animator
        void Hit()
        {
            if(target != null)
            {
                // Se a arma tiver projetil, ira lancar o projetil. Senao ele vai atacar
                if (currentWeapon.HasProjectile())
                {
                    currentWeapon.LauchProjectile(_rightHandTransform, _leftHandTransform, target, GetComponent<Collider>());
                }
                else
                {
                    target.TakeDamage(currentWeapon.GetWeaponDamage());
                }
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetWeaponRange();
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

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            if (weapon != null)
            {
                weapon.SpawnWeapon(_anim, _rightHandTransform, _leftHandTransform);
            }
        }
    }
}
