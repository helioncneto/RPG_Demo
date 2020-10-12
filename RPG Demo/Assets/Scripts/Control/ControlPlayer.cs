using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;

namespace RPG.Control
{
    public class ControlPlayer : MonoBehaviour
    {
        private Mover _mover;
        private Fighter _fighter;
        Health _playerHealth;

        enum CursorType
        {
            None,
            Combat,
            Movement
        }
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings;

        void Awake()
        {
            _mover = GetComponent<Mover>();
            if(_mover == null)
            {
                Debug.LogError("Mover is Null");
            }
            _fighter = GetComponent<Fighter>();
            if(_fighter == null)
            {
                Debug.LogError("Fighter is Null");
            }
            _playerHealth = GetComponent<Health>();
            if (_playerHealth == null)
            {
                Debug.LogError("Health is Null");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerHealth.IsDead()) return;
            if (CombatInteraction()) return;
            if (MovementInteraction())  return;

            SetCursor(CursorType.None);
        }

        

        private bool CombatInteraction()
        {
                RaycastHit[] hits = Physics.RaycastAll(GetRaycast());
                foreach(RaycastHit hit in hits)
                {
                    CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                    if (target != null)
                    {
                        
                            if (_fighter.CanAttack(target.gameObject))
                            {
                                if (Input.GetMouseButton(0))
                                {
                                    _fighter.Attack(target.gameObject);
                                }
                                SetCursor(CursorType.Combat);
                                return true;
                            }
                            else
                            {
                                continue;
                            }
                            
                        
                    }
                    else
                    {
                    continue;
                    }
                
                }
                return false;
            
        }

        private bool MovementInteraction()
        {
            
            bool isHit = Physics.Raycast(GetRaycast(), out RaycastHit hit);
            if (isHit)
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hit.point);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping cursorMap in cursorMappings)
            {
                if(cursorMap.type == type)
                {
                    return cursorMap;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetRaycast()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
