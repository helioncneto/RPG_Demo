using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    //Executa esse codigo em modo de edição
    [ExecuteAlways]
    public class Saveable : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier;
        // Dicionario onde sera salvo todos os objetos salvaveis
        // Sua principal função é verificar se existe UUID duplicado
        static Dictionary<string, Saveable> globalSaveable = new Dictionary<string, Saveable>();

        public string GetIdentifier()
        {
            return uniqueIdentifier;
        }

        public object GetStates()
        {
            //return new SerializableVector3(transform.position);
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.GetStates();
            }
            return state;
        }

        public void RestoreState(object state)
        {
            //Talvez seja melhor desabilitar o NavMeshAgent fora dessa função.
            //GetComponent<NavMeshAgent>().enabled = false;
            //SerializableVector3 position = (SerializableVector3)state;
            //transform.position = position.ToVector3();
            //GetComponent<NavMeshAgent>().enabled = true;
            //GetComponent<ActionScheduler>().CancelCurrentAction();
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeStr = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeStr))
                {
                    saveable.RestoreState(stateDict[typeStr]);
                }
            }
        }

// Essa parte do código não sera compilada, ela só serve para gerar UUIDs para objetos salváveis na cena
#if UNITY_EDITOR
        private void Update()
        {
            // Não executa em playing mode e se o objeto não estiver na cena
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            // Manupula um objecto serializado
            SerializedObject serializedObject = new SerializedObject(this);
            // Procura a propriedade (Variavel) de um objeto serializado
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                // Se o objeto não tiver um valor unico de identificação, é gerado um.
                property.stringValue = System.Guid.NewGuid().ToString();
                // Essa função garante que a propriedade seja aplicada.
                serializedObject.ApplyModifiedProperties();
            }

            globalSaveable[property.stringValue] = this;

        }

        private bool IsUnique(string candidate)
        {
            // Caso a chave não esteja no dicionario retorna true
            if (!globalSaveable.ContainsKey(candidate)) return true;
            // Caso a chave que esteja no dicionário seja ele mesmo, retorna true
            if (globalSaveable[candidate] == this) return true;
            // Caso o esse UUID seja de um objeto que foi destruido.
            if(globalSaveable[candidate] == null)
            {
                globalSaveable.Remove(candidate);
                return true;
            }
            // Quando um objeto pegou um UUID que já existe no dicionario, mas por algum motivo o objeto dono desse UUID não é
            // mais o dono desse UUID, ou seja, está desatualizado.
            if(globalSaveable[candidate].GetIdentifier() != candidate)
            {
                globalSaveable.Remove(candidate);
                return true;
            }
            // Senão false
            return false;
        }
#endif
    }
}
