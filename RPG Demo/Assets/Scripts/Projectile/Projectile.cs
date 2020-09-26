using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 3f;
    Health target;
    float projectileDamage;
    Collider shooter;

    private void Update()
    {
        if (target != null)
        {
            //Se colocar o lookAt aqui a flexa vai perseguir o alvo.
            //transform.LookAt(GetTargetPosition());
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
        if(other != shooter)
        {
            Health targetHealth = other.transform.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(projectileDamage);
            }
            Destroy(gameObject);
        }
        else
        {
            print("atirei em mim");
        }
    }
}


