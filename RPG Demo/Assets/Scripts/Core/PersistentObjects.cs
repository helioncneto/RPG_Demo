using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjects : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;
        static bool hasSpawned = false;

        private void Awake()
        {
            if (!hasSpawned) {
                SpawnPersistentObjects();
                hasSpawned = true;
            }
        }

        private void SpawnPersistentObjects()
        {
            GameObject pesistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(pesistentObject);
        }
    }
}
