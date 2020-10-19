using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject target;

        public void DestroyTarget()
        {
            Destroy(target);
        }
    }
}
