using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {

        private NavMeshAgent _navmesh;
        private Animator _anim;
        private ActionScheduler _scheduler;
        [SerializeField] float _maxSpeed = 5.66f;

        void Awake()
        {
            _navmesh = GetComponent<NavMeshAgent>();
            if (_navmesh == null)
            {
                Debug.LogError("NavMesh is Null");
            }

            _anim = GetComponent<Animator>();
            if (_anim == null)
            {
                Debug.LogError("Animator is Null");
            }

            _scheduler = GetComponent<ActionScheduler>();
            if (_scheduler == null)
            {
                Debug.LogError("ActionScheduler is Null");
            }
        }

        void Update()
        {
            UpdateAnimator();

            // Minha implementação
            //_anim.SetFloat("movingForward", _navmesh.velocity.magnitude);

            // Desenhar o Ray para propósito de debug
            //Debug.DrawRay(lastRay.origin, lastRay.direction * 100);

            // Variaveis de distância
            //Debug.Log("Distancia faltante " + _navmesh.remainingDistance);
            //Debug.Log("Distancia parada " + _navmesh.stoppingDistance);
        }

        void UpdateAnimator()
        {
            Vector3 velocity = _navmesh.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _anim.SetFloat("movingForward", speed);

        }

        public void MoveToDestination(Vector3 destination, float speedFraction = 1f)
        {
            //_navmesh.destination = destination;
            _navmesh.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _navmesh.SetDestination(destination);
            _navmesh.isStopped = false;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction = 1f)
        {
            _scheduler.StartAction(this);
            MoveToDestination(destination, speedFraction);
        }

        public void Cancel()
        {
            _navmesh.isStopped = true;
        }

        public object GetStates()
        {
            //Talvez seja melhor desabilitar o NavMeshAgent fora dessa função.
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            //Talvez seja melhor desabilitar o NavMeshAgent fora dessa função.
            _navmesh.enabled = false;
            SerializableVector3 position = (SerializableVector3) state;
            transform.position = position.ToVector3();
            _navmesh.enabled = true;
            _scheduler.CancelCurrentAction();
        }
    }
}
