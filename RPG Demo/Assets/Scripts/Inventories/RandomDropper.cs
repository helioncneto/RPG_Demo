using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Stats;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far the pickkups can be dropped")]
        [SerializeField] float distance = 1;
        [SerializeField] DropLibrary dropLibrary;

        // Constants
        const int ATTEMPTS = 30;
        const float TOL = 0.1f;

        public void RandomDrop()
        {
            var baseStat = GetComponent<BaseStats>();
            var drops = dropLibrary.GetRandomDrops(baseStat.GetLevel());

            foreach(var drop in drops)
            {
                print(drop.item);
                DropItem(drop.item, drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            // Aqui vamos tentar escolher um lugar aleatorio em que haja navmesh, se tentar por 30x e não conseguir, entao instanciar na posição do GmaeObject
            for(int i = 0; i <= ATTEMPTS; i++)
            {
                // insideUnitSphere: Retorna um ponto aleatório em uma esfera com o raio igual a um
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * distance;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, TOL, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}
