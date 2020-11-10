using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> lookupNode = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if( dialogueNodes.Count <= 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = Guid.NewGuid().ToString();
                dialogueNodes.Add(rootNode);
            }
        }
#endif
        //É chamado a cada mudança do ScriptableObject
        private void OnValidate()
        {
            lookupNode.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                lookupNode[node.uniqueID] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return dialogueNodes;
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
            DialogueNode newNode = new DialogueNode();
            newNode.uniqueID = Guid.NewGuid().ToString();
            dialogueNodes.Add(newNode);
            parentNode.child.Add(newNode.uniqueID);
            lookupNode[newNode.uniqueID] = newNode;
        }
    }
}
