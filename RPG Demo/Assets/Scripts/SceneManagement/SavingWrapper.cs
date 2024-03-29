﻿using RPG.Control;
using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "save";
        SavingSystem _savingSystem;
        [SerializeField] float _fadeInTime = .3f;
        [SerializeField] float _waitForFadeIn = 1f;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return _savingSystem.LoadLastScene(saveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            // Retira o controle do jogador assim que a cena é carregada
            ControlPlayer playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<ControlPlayer>();
            playerController.enabled = false;

            yield return new WaitForSeconds(_waitForFadeIn);
            fader.FadeIn(_fadeInTime);
            playerController.enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Load()
        {
            _savingSystem.Load(saveFile);
        }

        public void Save()
        {
            _savingSystem.Save(saveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(saveFile);
        }
    }
}
