using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjects : MonoBehaviour
    {
        // CONFIG DATA
        [Tooltip("This prefab will only be spawned once and persisted between " +
        "scenes.")]
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
