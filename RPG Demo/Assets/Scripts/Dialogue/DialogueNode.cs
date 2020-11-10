using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueID;
        [TextArea] public string text;
        public string[] child;
        public Rect rect = new Rect(0, 0, 200, 100);
    }

}