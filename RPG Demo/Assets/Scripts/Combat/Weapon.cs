using RPG.Core;
using System;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] float weaponDamage = 25f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHandWeapon = true;
        [SerializeField] Projectile projectile;

        const string weaponName = "Weapon";

        public void SpawnWeapon(Animator animator, Transform rightHandTransform, Transform leftHandTransform)
        {
            if(weaponPrefab != null)
            {
                DestroyOldWeapon(rightHandTransform, leftHandTransform);
                // Ira instanciar a arma na mao do player
                GameObject weapon = Instantiate(weaponPrefab, GetHandTransform(rightHandTransform, leftHandTransform));
                weapon.name = weaponName;
            }
  
            if (animatorOverride != null)
            {
                // verifica se vai ter animator overrid
                animator.runtimeAnimatorController = animatorOverride;
            }

        }

        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform oldWeapon = rightHandTransform.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHandTransform.Find(weaponName);
            }
            if (oldWeapon == null) return;
            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile()
        {
            // verifica se essa arma possui um projetil
            return projectile != null;
        }

        public void LauchProjectile(Transform rightHand, Transform leftHand, Health target, Collider shooter)
        {
            // Irá instanciar o projetil
            Projectile spawnedProjectile = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            // seta o alvo
            spawnedProjectile.SetTarget(target, weaponDamage, shooter);

        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            if (isRightHandWeapon)
            {
                return rightHand;
            }
            else
            {
                return leftHand;
            }
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }
    }
}