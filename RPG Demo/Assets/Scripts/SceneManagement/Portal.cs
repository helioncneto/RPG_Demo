using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum Destination
        {
            A, B, C, D, E
        }

        [SerializeField] Destination destintion;
        [SerializeField] int _sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float _fadeInTime = 0.5f;
        [SerializeField] float _waitTime = 0.5f;
        [SerializeField] float _fadeOutTime = 0.5f;
        SavingWrapper _save;

        private void Start()
        {
            _save = FindObjectOfType<SavingWrapper>().GetComponent<SavingWrapper>();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        IEnumerator Transition()
        {
            if(_sceneToLoad < 0)
            {
                Debug.LogError("There is no scene to load");
                yield break;
            }

            
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();
            // Desabilitar controle do jogador
            ControlPlayer playerCurrent = GameObject.FindGameObjectWithTag("Player").GetComponent<ControlPlayer>();
            playerCurrent.enabled = false;

            yield return fader.FadeOut(_fadeOutTime);
            _save.Save();
            yield return SceneManager.LoadSceneAsync(_sceneToLoad);
            ControlPlayer playerNew = GameObject.FindGameObjectWithTag("Player").GetComponent<ControlPlayer>();
            playerNew.enabled = false;

            _save.Load();
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            // Aqui salva de novo para gravar a nova posição se player
            _save.Save();
            yield return new WaitForSeconds(_waitTime);
            // Essa linha sem o yield return não irá esperar o fade acabar para liberar o controle
            fader.FadeIn(_fadeInTime);
            
            // Retorna o controle para o jogador
            playerNew.enabled = true;

            Destroy(gameObject);
        }


        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destintion == destintion) return portal; ;
                
            }
            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;

        }
    }
}
