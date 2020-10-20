using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        void LateUpdate()
        {
            // Faz com que o objeto aponte para a camera todo tempo
            transform.forward = Camera.main.transform.forward;
        }
    }

}