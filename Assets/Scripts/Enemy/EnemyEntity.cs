using System;
using System.Threading.Tasks;
using Configs;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyEntity : MonoBehaviour, IDamaged
    {
        public Action onDeath;

        [SerializeField] private NavMeshAgent navMeshAgent;

        [SerializeField] private BaseParams _baseParams;

        [SerializeField] private Transform player;
        
        public void Init(BaseParams baseParams)
        {
            _baseParams = baseParams;
            navMeshAgent.stoppingDistance = _baseParams.attackRange;
        }

        private void Start()
        {
            MoveToPoint(player);
        }
        
        private async void MoveToPoint(Transform target)
        {
            navMeshAgent.SetDestination(target.position);
            await Task.Delay(100);
            while (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance + .05f)
            { 
                navMeshAgent.SetDestination(target.position);
                transform.LookAt(target);
                await Task.Yield();
            }
        }

        public void Hit(float damage)
        {
            _baseParams.heath -= damage;

            Debug.Log(_baseParams.heath);
            
            if (_baseParams.heath <= 0)
            {
                Death();
            }
        }
        
        private void Death()
        {
            Debug.Log("сдох");
            onDeath?.Invoke();
        }
    }
}