using RPG.Attributes;
using System;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] Weapon weaponPrefab;
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] float weaponDamage = 25f;
        [SerializeField] float weaponPercentageBonus = 0;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHandWeapon = true;
        [SerializeField] Projectile projectile;


        const string weaponName = "Weapon";

        public Weapon SpawnWeapon(Animator animator, Transform rightHandTransform, Transform leftHandTransform)
        {
            // Remover a arma antiga, mesmo que a arma atual nao tenha um gameobject
            DestroyOldWeapon(rightHandTransform, leftHandTransform);
            Weapon weapon = null;
            if (weaponPrefab != null)
            {
                // Ira instanciar a arma na mao do player
                weapon = Instantiate(weaponPrefab, GetHandTransform(rightHandTransform, leftHandTransform));
                weapon.gameObject.name = weaponName;
            }
            // Aqui chaca se o animator atual e um controlleroveride. Se nao for retorna null
            var ControllerOveride = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                // verifica se vai ter animator overrid
                animator.runtimeAnimatorController = animatorOverride;
            } else if(ControllerOveride != null)
            {
                // Se o ControllerOveride nao for nulo, entao o animator atual e um override, entao deve trocar para o controller padrao.
                animator.runtimeAnimatorController = ControllerOveride.runtimeAnimatorController;
            }
            return weapon;
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

        public void LauchProjectile(Transform rightHand, Transform leftHand, Health target, Collider shooter, float calculatedDamage)
        {
            // Irá instanciar o projetil
            Projectile spawnedProjectile = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            // seta o alvo
            spawnedProjectile.SetTarget(target, calculatedDamage, shooter);

        }

        public float GetPercentagteBonus()
        {
            return weaponPercentageBonus;
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