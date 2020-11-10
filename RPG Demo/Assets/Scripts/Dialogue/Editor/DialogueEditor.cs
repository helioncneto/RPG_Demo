using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue;
        GUIStyle nodeStyle;
        DialogueNode draggingNode;
        Vector2 draggingOffset;

        [MenuItem("Window/Dialog Editor")]
        public static void Initialize()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
        
        // Cuidado ao usar OnOpenAsset, pois todo asset aberto vai rodar esse metodo, por isso devemos checar se o metodo esta sendo chamado pelo asset correto
        // Essa anotation envia dois a tres parametros (instanceID, line e row). No nosso caso so usamos o instanceID
        [OnOpenAsset(1)]
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

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            //nodeStyle.normal.textColor = Color.white;
            // Valores conseguidos atraves de testes
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if(dialogue != null)
            {
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConections(node);
                }
                // Existem dois foreach para impedir que as linhas passem por cima dos nós
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    OnGUINode(node);
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                }

            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Dragging");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
                //Undo.RecordObject(selectedDialogue, "Drag the node");
                //selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;
            } 
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            // Começa a verificar se há mudança no Editor
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
            string newText = EditorGUILayout.TextField(node.text);
            string newID = EditorGUILayout.TextField(node.uniqueID);
            // EndChangeCHeck verifica se alguma mudança foi feita, desde BeginChance, se sim retorna true
            if (EditorGUI.EndChangeCheck())
            {
                // Grava oq foi feito e caso aperte ctrl + z, seja refeito
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                node.text = newText;
                node.uniqueID = newID;
                // Salva oq foi editado no Editor no Scriptanle object
                //EditorUtility.SetDirty(selectedDialogue);
            }
            GUILayout.EndArea();
        }

        private void DrawConections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPositionOffset = endPosition - startPosition;
                controlPositionOffset.y = 0;
                controlPositionOffset.x *= 0.8f;

                Vector3 startTangent = startPosition + controlPositionOffset;
                Vector3 endTangent = endPosition - controlPositionOffset;
                Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 position)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.rect.Contains(position))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}
