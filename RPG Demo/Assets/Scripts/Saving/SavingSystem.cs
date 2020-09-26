using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> dictFile = LoadFile(saveFile);
            if (dictFile.ContainsKey("lastSceneIndex"))
            {
                int sceneIndex = (int)dictFile["lastSceneIndex"];
                if (sceneIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    yield return SceneManager.LoadSceneAsync(sceneIndex);
                }
                RestoreState(dictFile);
            }
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private void SaveFile(string saveFile, Dictionary<string, object> state)
        {

            string path = GetPathSaveFile(saveFile);
            // Fecha o arquivo mesmo se houver uma exceção no meio do caminho
            // Metodo seguro para garantir que o arquivo será fechado
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using(FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);  
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (Saveable entity in FindObjectsOfType<Saveable>())
            {
                string id = entity.GetIdentifier();
                if (state.ContainsKey(id))
                {
                    entity.RestoreState(state[id]);
                }
                
            }

        }

        private void CaptureState(Dictionary<string, object> stateDictionary)
        {
            foreach(Saveable entity in FindObjectsOfType<Saveable>())
            {
                stateDictionary[entity.GetIdentifier()] = entity.GetStates();
            }

            stateDictionary["lastSceneIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private string GetPathSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}
