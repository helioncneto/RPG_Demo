using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class ControlPlayer : MonoBehaviour
    {
        private Mover _mover;
        private Fighter _fighter;
        Health _playerHealth;

        void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();

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
        }

        private bool CombatInteraction()
        {
                RaycastHit[] hits = Physics.RaycastAll(GetRaycast());
                foreach(RaycastHit hit in hits)
                {
                    CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                    if (target != null)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if (_fighter.CanAttack(target.gameObject))
                            {
                                _fighter.Attack(target.gameObject);
                            }
                            else
                            {
                                continue;
                            }
                            
                        }
                    }
                    else
                    {
                    continue;
                    }
                    return true;
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
                return true;
            }
            return false;
        }

        private static Ray GetRaycast()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
