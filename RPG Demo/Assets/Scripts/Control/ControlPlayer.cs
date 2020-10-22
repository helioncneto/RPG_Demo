using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class ControlPlayer : MonoBehaviour
    {
        private Mover _mover;
        private Fighter _fighter;
        Health _playerHealth;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings;
        [SerializeField] float naveMeshDistance = 1f;
        [SerializeField] float raycastComponentRadius = 0.5f;
        //[SerializeField] float maxDistance = 40f;

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
            if (InteractWithUI())
            {
                SetCursor(CursorType.UI);
                return;
            }
            if (_playerHealth.IsDead())
            {
                // Mudar o cursor em um futuro, talvez
                SetCursor(CursorType.None);
                return;
            }
            if (ComponentInteraction()) return;
            if (MovementInteraction())  return;

            SetCursor(CursorType.None);
        }

        private bool ComponentInteraction()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetRaycast(), raycastComponentRadius);
            float[] distance = new float[hits.Length];
            for(int i = 0; i < distance.Length; i++)
            {
                distance[i] = hits[i].distance;
            }
            Array.Sort(distance, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            // Verifica se esta apontando para uma UI
            return EventSystem.current.IsPointerOverGameObject();
        }

        private bool MovementInteraction()
        {
            bool isHit = RaycastNavMesh(out Vector3 target);    
            if (isHit)
            {
                if (!_mover.CanMoveTo(target)) return false;
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            bool hasHit = Physics.Raycast(GetRaycast(), out RaycastHit hit);
            if (!hasHit) return false;

            target = hit.point;
            bool hasNavMesh = NavMesh.SamplePosition(target, out NavMeshHit navMeshHit, naveMeshDistance, NavMesh.AllAreas);
            if (!hasNavMesh) return false;

            target = navMeshHit.position;
            //NavMeshPath navMeshPath = new NavMeshPath();
            //bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, navMeshPath);
            //if (!hasPath) return false;
            //if (navMeshPath.status != NavMeshPathStatus.PathComplete) return false;
            //if (CalculateDistante(navMeshPath) > maxDistance) return false; 
            return true;
            
        }

        //private float CalculateDistante(NavMeshPath path)
        //{
        //    float total = 0f;
        //    if (path.corners.Length < 2) return total;
        //    for(int i = 0; i < path.corners.Length -1; i++)
        //    {
        //        total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        //    }
        //    return total;
        //}

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
