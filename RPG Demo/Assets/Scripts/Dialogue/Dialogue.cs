using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        public int editorSize = 4000;
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> lookupNode = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if( dialogueNodes.Count <= 0)
            {
                CreateNode(null);
            }
        }
#endif
        //É chamado a cada mudança do ScriptableObject
        private void OnValidate()
        {
            lookupNode.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                lookupNode[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return dialogueNodes;
        }

        public int GetNodesAmount()
        {
            return dialogueNodes.Count;
        }

        public DialogueNode GetRootNode()
        {
            return dialogueNodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach(string childNode in parentNode.child)
            {
                if (lookupNode.ContainsKey(childNode))
                {
                    yield return lookupNode[childNode];
                }
                else
                {
                    Debug.LogError("The child " + childNode + " does not exist");
                }
            }

        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Node");
            dialogueNodes.Add(newNode);
            if(parentNode != null)
            {
                parentNode.child.Add(newNode.name);
            }
            //lookupNode[newNode.uniqueID] = newNode;
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
            OnValidate();
            UnlinkChildren(nodeToDelete);
        }

        private void UnlinkChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.child.Remove(nodeToDelete.name);
            }
        }
    }
}
