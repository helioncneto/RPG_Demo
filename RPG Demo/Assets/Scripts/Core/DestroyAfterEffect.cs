using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject target;
        ParticleSystem particle;

        void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {

            if (!particle.IsAlive())
            {
                if (target != null)
                {
                    Destroy(target);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
