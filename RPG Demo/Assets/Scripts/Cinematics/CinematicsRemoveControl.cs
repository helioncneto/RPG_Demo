using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicsRemoveControl : MonoBehaviour
    {
        PlayableDirector _director;
        GameObject _player;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            if (_director == null)
            {
                Debug.LogError("Playable Director is null");
            }
            _player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            _director.played += DisableControl;
            _director.stopped += EnableControl;
        }

        private void OnDisable()
        {
            _director.played -= DisableControl;
            _director.stopped -= EnableControl;
        }

        void EnableControl(PlayableDirector director)
        {
            //print("Enable Control");
            _player.GetComponent<ControlPlayer>().enabled = true;
        }

        void DisableControl(PlayableDirector director)
        {
            //print("Disable Control");
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<ControlPlayer>().enabled = false;
        }
    }
}
