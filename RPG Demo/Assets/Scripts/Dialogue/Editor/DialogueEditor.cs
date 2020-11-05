using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [MenuItem("Window/Dialog Editor")]
        public static void Initialize()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
        
        // Cuidado ao usar OnOpenAssetAttribute, pois todo asset aberto vai rodar esse metodo, por isso devemos checar se o metodo esta sendo chamado pelo asset correto
        [OnOpenAssetAttribute(1)]
        public static bool OpenDialogueEditor(int instanceID, int line)
        {
            // Retornara Null se nao for DialogEditor
            Dialogue instance = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(instance != null)
            {
                Initialize();
                return true;
            }
            return false;
        }
    }
}
