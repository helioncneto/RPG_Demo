using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        public string uniqueID;
        [TextArea] public string text;
        public List<string> child = new List<string>();
        public Rect rect = new Rect(0, 0, 200, 100);
    }

}