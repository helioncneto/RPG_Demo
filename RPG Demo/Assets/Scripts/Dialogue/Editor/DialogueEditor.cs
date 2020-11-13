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
        //Usamos NoNSerializes, pois o EditorWindow serializa todas suas variáveis e isso acaba transformando o creatingNode em not null, fazendo com que novos nós sejam criados.
        Dialogue selectedDialogue;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        DialogueNode draggingNode;
        [NonSerialized]
        Vector2 draggingOffset;

        // Criamos esse objeto para que o novo nó sejá criado depois do foreach loop
        [NonSerialized]
        DialogueNode creatingNode;
        [NonSerialized]
        DialogueNode deletingNode;
        [NonSerialized]
        DialogueNode linkinParentNode;
        [NonSerialized]
        bool draggingCanvas;
        [NonSerialized]
        Vector2 canvasOffset;
        Vector2 scrollPosition;

        // CONSTANTS
        const int backgroundSize = 50;

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
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                Rect canvas = GUILayoutUtility.GetRect(selectedDialogue.editorSize, selectedDialogue.editorSize);
                // O background é colocado como Tiles, entao o Height e Width de texcords deve conter quantos backgrounds serão colocados
                // Que no caso foi o tamanho do canvas dividido pelo tamanho do backgrouns
                Rect texCoords = new Rect(0, 0, selectedDialogue.editorSize / backgroundSize, selectedDialogue.editorSize / backgroundSize);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConections(node);
                }
                // Existem dois foreach para impedir que as linhas passem por cima dos nós
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();
            }
            if(creatingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Creating a node");
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }
            if (deletingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Deleting a node");
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                } else
                {
                    draggingCanvas = true;
                    draggingOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }

            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Dragging");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                Undo.RecordObject(selectedDialogue, "Dragging Canvas");
                scrollPosition = draggingOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
                //Undo.RecordObject(selectedDialogue, "Drag the node");
                //selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            // Começa a verificar se há mudança no Editor
            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextField(node.text);
            // EndChangeCHeck verifica se alguma mudança foi feita, desde BeginChance, se sim retorna true
            if (EditorGUI.EndChangeCheck())
            {
                // Grava oq foi feito e caso aperte ctrl + z, seja refeito
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                node.text = newText;
                // Salva oq foi editado no Editor no Scriptanle object
                //EditorUtility.SetDirty(selectedDialogue);
            }

            GUILayout.BeginHorizontal();
            GetAddButton(node);
            GetLinkButton(node);
            GetDeleteButton(node);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void GetAddButton(DialogueNode node)
        {
            if (GUILayout.Button("Add"))
            {
                creatingNode = node;
            }
        }

        private void GetDeleteButton(DialogueNode node)
        {
            if(selectedDialogue.GetNodesAmount() > 1)
            {
                if (GUILayout.Button("Del"))
                {
                    deletingNode = node;
                }
            }
            
        }

        private void GetLinkButton(DialogueNode node)
        {
            if (linkinParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkinParentNode = node;
                }
            }
            else
            {
                if (linkinParentNode != node)
                {
                    if (!linkinParentNode.child.Contains(node.name))
                    {
                        if (GUILayout.Button("child"))
                        {

                            Undo.RecordObject(selectedDialogue, "Linking nodes");
                            linkinParentNode.child.Add(node.name);
                            linkinParentNode = null;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Unlink"))
                        {
                            Undo.RecordObject(selectedDialogue, "Linking nodes");
                            linkinParentNode.child.Remove(node.name);
                            linkinParentNode = null;
                        }
                    }

                }
                else if (linkinParentNode == node)
                {
                    if (GUILayout.Button("Cancel"))
                    {
                        linkinParentNode = null;
                    }
                }
            }
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
