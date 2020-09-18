using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class ControlAI : MonoBehaviour
    {
        [SerializeField] private float _pursuitRange = 5f;
        [SerializeField] float _suspiciousTime = 4f;
        [SerializeField] PatrolPaths patrolPaths;
        [Range(0, 1)]
        [SerializeField] private float _PatrolFractionSpeed = 0.5f;

        GameObject _player;
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        private ActionScheduler _scheduler;
        Vector3 _guardPosition;
        private float _lastTimePlayerSaw = Mathf.Infinity;
        private float _waypointTolerance = 1f;
        private int _currentWaypoint = 0;
        private float _timeToHoldInPatrol = Mathf.Infinity;
        private float _timeInTheWaypoint = Mathf.Infinity;
        private float _minTimeToWait = .5f;
        private float _maxTimeToWait = 4f;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _scheduler = GetComponent<ActionScheduler>();
            
            _guardPosition = transform.position;
            _timeToHoldInPatrol = GetTimeToWait(_minTimeToWait, _maxTimeToWait);
        }

        private void Update()
        {
            if (_health.IsDead()) return;
            if (IsPlayerInRange() && _fighter.CanAttack(_player))
            {
                _lastTimePlayerSaw = 0;
                AttackBehavior();

            }
            else if (_lastTimePlayerSaw < _suspiciousTime)
            {
                SuspiciousBehavior();
            }
            else
            {
                PatrolBehavior();
            }
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = _guardPosition;
            if (patrolPaths != null)
            {
                if (atWaypoint())
                {
                    if(_timeInTheWaypoint > _timeToHoldInPatrol)
                    {
                        _currentWaypoint = GetNextWaypoints();
                        _timeInTheWaypoint = 0;
                        _timeToHoldInPatrol = GetTimeToWait(_minTimeToWait, _maxTimeToWait);
                    }
                    else
                    {
                        _timeInTheWaypoint += Time.deltaTime;
                    }
                    
                }
                nextPosition = GetCurrentPosition();
            }

            _mover.StartMoveAction(nextPosition, _PatrolFractionSpeed);
        }

        private float GetTimeToWait(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        private Vector3 GetCurrentPosition()
        {
            return patrolPaths.GetWaypoint(_currentWaypoint);
        }

        private int GetNextWaypoints()
        {
            return (_currentWaypoint + 1) % patrolPaths.GetTotalWaypoints();
        }

        private bool atWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentPosition());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void SuspiciousBehavior()
        {
            _scheduler.CancelCurrentAction();
            _lastTimePlayerSaw += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            _fighter.Attack(_player);
        }

        private bool IsPlayerInRange()
        {
            return Vector3.Distance(transform.position, _player.transform.position) < _pursuitRange;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _pursuitRange);
        }
    }
}
