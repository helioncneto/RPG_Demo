﻿using System.Collections;
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
        [SerializeField] private float _distanceRange = 2f;
        ActionScheduler _scheduler;
        private Animator _anim;
        [SerializeField] float _breath = 1f;
        [SerializeField] float _damage = 25f;
        float _currentTime = 0;
        float _attackTime = 0;

        private void Start()
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
                target.TakeDamage(_damage);
            }
        }

        private bool GetInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < _distanceRange;
        }

        public void Cancel()
        {
            _anim.SetTrigger("stopAttack");
            target = null;
        }

        public void Attack(GameObject combatTarget)
        {
            _scheduler.StartAction(this);
            target = combatTarget.transform.GetComponent<Health>();
        }

    }
}
