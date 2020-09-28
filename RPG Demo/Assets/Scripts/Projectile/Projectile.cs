using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed = 3f;
        Health target;
        float projectileDamage;
        Collider shooter;
        [SerializeField] bool isHomingProjectile;
        [SerializeField] GameObject _hitImpact;
        [SerializeField] float _maxLifeTime = 10f;
        [SerializeField] GameObject[] _destroyAfterImpact;
        [SerializeField] float _timeAfterImpact = 0.2f;

        private void Start()
        {
            Destroy(gameObject, _maxLifeTime);
        }

        private void Update()
        {
            if (target != null)
            {
                if (isHomingProjectile && !target.IsDead())
                {
                    //Se colocar o lookAt aqui a flexa vai perseguir o alvo.
                    transform.LookAt(GetTargetPosition());
                }
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
            }


        }

        public void SetTarget(Health setTarget, float damage, Collider shooterCollider)
        {
            target = setTarget;
            projectileDamage = damage;
            shooter = shooterCollider;
            transform.LookAt(GetTargetPosition());
        }

        private Vector3 GetTargetPosition()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            else
            {
                // Retorna a posiçãod o meio do capsule collider
                Vector3 offset = Vector3.up * targetCapsule.height / 2;
                return target.transform.position + offset;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != shooter)
            {
                Health targetHealth = other.transform.GetComponent<Health>();
                if (target.IsDead()) return;
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(projectileDamage);
                }

                // Para o movimento do projetil para que o tail possa se destruir aos poucos
                _speed = 0;

                if (_hitImpact != null)
                {
                    Instantiate(_hitImpact, GetTargetPosition(), transform.rotation);

                }
                if (_destroyAfterImpact.Length > 0)
                {
                    foreach (GameObject destroy in _destroyAfterImpact)
                    {
                        Destroy(destroy);
                    }
                }

                Destroy(gameObject, _timeAfterImpact);
            }
        }
    }
}