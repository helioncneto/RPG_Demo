using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class PickupWeapon : MonoBehaviour
    {
        [SerializeField] Weapon weapon;
        [SerializeField] float _timeForRespawn = 5;

        Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            if(_collider == null)
            {
                Debug.LogError("The Collider is Null");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(WaitForRespawn(_timeForRespawn));
                //Destroy(gameObject);
            }
        }

        IEnumerator WaitForRespawn(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool show)
        {
            _collider.enabled = show;
            transform.GetChild(0).gameObject.SetActive(show);
        }
    }
}
