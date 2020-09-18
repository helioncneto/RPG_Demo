using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _isPlayed;
        private PlayableDirector _director;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            if(_director == null)
            {
                Debug.LogError("Playable Director is null");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isPlayed)
            {
                _director.Play();
                _isPlayed = true;
            }
        }
    }
}
